using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace ChatManager.Models
{
    ///////////////////////////////////////////////////////////////
    // Ce patron de classe permet de stocker dans un fichier JSON
    // une collection d'objects. Ces derniers doivent posséder
    // la propriété int Id {get; set;}
    // Après l'instanciation il faut invoquer la méthode Init
    // pour fournir le chemin d'accès du fichier JSON.
    // Voir dans global.asax
    //
    // Author : Nicolas Chourot
    ///////////////////////////////////////////////////////////////
    public class Repository<T>
    {
        #region "Méthodes et propritées privées"
        // Pour indiquer si une transaction est en cours
        static bool TransactionOnGoing = false;
        // utilisé pour prévenir des conflits entre processus
        static private readonly Mutex mutex = new Mutex();
        // cache des données du fichier JSON
        private List<T> dataList;
        // chemin d'accès absolue du fichier JSON
        private string FilePath;
        // Numéro de serie des données
        private string _SerialNumber;
        // retourne la valeur de l'attribut attributeName de l'intance data de classe T

        private object GetAttributeValue(T data, string attributeName)
        {
            return data.GetType().GetProperty(attributeName).GetValue(data, null);
        }
        // affecter la valeur de l'attribut attributeName de l'intance data de classe T
        private void SetAttributeValue(T data, string attributeName, object value)
        {
            data.GetType().GetProperty(attributeName).SetValue(data, value, null);
        }
        // Vérifier si l'attribut attributeName est présent dans la classe T
        private bool AttributeNameExist(string attributeName)
        {
            return (Activator.CreateInstance(typeof(T)).GetType().GetProperty(attributeName) != null);
        }

        // retourne la valeur de l'attribut Id d'une instance de classe T
        private int Id(T data)
        {
            return (int)GetAttributeValue(data, "Id");
        }
        // Lecture du fichier JSON et conservation des données dans la cache dataList
        private void ReadFile()
        {
            MarkHasChanged();
            if (dataList != null)
            {
                dataList.Clear();
            }
            try
            {
                using (StreamReader sr = new StreamReader(FilePath))
                {
                    dataList = JsonConvert.DeserializeObject<List<T>>(sr.ReadToEnd());
                }
                if (dataList == null)
                {
                    dataList = new List<T>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Mise à jour du fichier JSON avec les données présentes dans la cache dataList
        private void UpdateFile()
        {
            if (dataList != null)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(dataList));
                    }
                    ReadFile();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        // retourne le prochain Id disponible
        private int NextId()
        {
            int maxId = 0;
            if (dataList == null)
                return 1;
            foreach (var data in dataList)
            {
                int Id = this.Id(data);
                if (Id > maxId)
                    maxId = Id;
            }
            return maxId + 1;
        }
        #endregion

        #region "Méthodes publiques"
        // constructeur
        public Repository()
        {
            dataList = new List<T>();
            try
            {
                // s'assurer que la propriété int Id {get; set;} est belle et bien dans la classe T
                var idExist = AttributeNameExist("Id");
                if (!idExist)
                    throw new Exception("The class Repository cannot work with type that does not contain an attribute named Id of type int.");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool HasChanged
        {
            get
            {
                string key = this.GetType().Name;
                if (((string)HttpContext.Current.Session[key] != _SerialNumber))
                {
                    HttpContext.Current.Session[key] = _SerialNumber;
                    return true;
                }
                return false;
            }
        }
        public void BeginTransaction()
        {
            mutex.WaitOne();
            TransactionOnGoing = true;
        }
        public void EndTransaction()
        {
            TransactionOnGoing = false;
            mutex.ReleaseMutex();
        }
        // Init : reçoit le chemin d'accès absolue du fichier JSON
        // Cette méthode doit avoir été appelée avant tout
        public virtual void Init(string filePath)
        {
            if (!TransactionOnGoing) mutex.WaitOne();
            try
            {
                FilePath = filePath;
                if (string.IsNullOrEmpty(FilePath))
                {
                    throw new Exception("FilePath not set exception");
                }
                if (!File.Exists(FilePath))
                {
                    using (StreamWriter sw = File.CreateText(FilePath))
                    {
                        sw.Close();
                    };
                }
                ReadFile();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!TransactionOnGoing) mutex.ReleaseMutex();
            }
        }
        public virtual void MarkHasChanged()
        {
            _SerialNumber = Guid.NewGuid().ToString();
        }

        // Méthodes CRUD
        // Read
        public T Get(int Id)
        {
            foreach (var data in dataList)
            {
                int dataId = this.Id(data);
                if (dataId == Id)
                    return data;
            }
            return default(T);
        }
        public List<T> ToList()
        {
            return dataList;
        }
        // Create
        public virtual int Add(T data)
        {
            int newId = 0;
            if (!TransactionOnGoing) mutex.WaitOne(); // attendre la conclusion d'un appel concurrant
            try
            {
                newId = NextId();
                SetAttributeValue(data, "Id", newId);
                dataList.Add(data);
                UpdateFile();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!TransactionOnGoing) mutex.ReleaseMutex();
            }
            return newId;
        }
        // Update
        public virtual bool Update(T data)
        {
            bool success = false;
            if (!TransactionOnGoing)
                mutex.WaitOne();
            try
            {
                T dataToUpdate = Get(Id(data));
                if (dataToUpdate != null)
                {
                    int index = dataList.IndexOf(dataToUpdate);
                    dataList[index] = data;
                    UpdateFile();
                    success = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!TransactionOnGoing)
                    mutex.ReleaseMutex();
            }
            return success;
        }
        // Delete
        public virtual bool Delete(int Id)
        {
            bool success = false;
            if (!TransactionOnGoing)
                mutex.WaitOne();
            try
            {
                T dataToDelete = Get(Id);

                if (dataToDelete != null)
                {
                    int index = dataList.IndexOf(dataToDelete);
                    dataList.RemoveAt(index);
                    UpdateFile();
                    success = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (!TransactionOnGoing)
                    mutex.ReleaseMutex();
            }
            return success;
        }
        #endregion
    }
}