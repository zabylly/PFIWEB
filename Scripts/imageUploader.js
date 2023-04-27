﻿/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Author: Nicolas Chourot
// February 2023
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// This script generate necessary html control in order to offer an image uploader.
// Also it include validation rules to avoid submission on empty file and excessive image size.
//
// This script is dependant of jquery and jquery validation.
//
//  Any <div> written as follow will contain an image file uploader :
//
//  <div class='imageUploader' id='data_Id' controlId = 'controlId' imageSrc='image url'> </div>
//  <span class="field-validation-valid text-danger" data-valmsg-for="controlId" data-valmsg-replace="true"></span>
//
//  If data_Id = 0 the file not empty validation rule will be applied
//
//  Example:
//
//  With the following:
//  <div class='imageUploader' id='0' controlId='PhotoImageData' imageSrc='/Photos/No_image.png' waitingImage="/PhotosManager/Photos/Loading_icon.gif"> </div>
//  <span class="field-validation-valid text-danger" data-valmsg-for="PhotoImageData" data-valmsg-replace="true"></span>
//
//  We obtain:
//  <div class="imageUploader" id="0" 
//       controlid="PhotoImageData"
//       imagesrc="/Photos/No_image.png" 
//       waitingImage = "/PhotosManager/Photos/Loading_icon.gif" >
//
//      <!-- Image uploaded -->
//      <img id="PhotoImageData_UploadedImage"
//           name="PhotoImageData_UploadedImage"
//           class="UploadedImage"
//           src="/Photos/No_image.png">
//
//      <!-- hidden file uploader -->
//      <input id="PhotoImageData_ImageUploader"
//             type="file"
//             style="visibility:hidden; height:0px;"
//             accept="image/jpeg,image/gif,image/png,image/bmp">
//  
//      <!-- hidden input uploaded imageData container -->
//      <input style="visibility:hidden;height:0px;"
//             class="fileUploadedExistRule fileUploadedSizeRule input-validation-error"
//             id="PhotoImageData"
//             name="PhotoImageData"
//             createmode="true"
//             waitingImage="/PhotosManager/Photos/Loading_icon.gif">
//  </div>
//  <span class="field-validation-valid text-danger" data-valmsg-for="PhotoImageData" data-valmsg-replace="true"></span>
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////


// Error messages
//let missingFileErrorMessage = "You must select an image file.";
//let tooBigFileErrorMessage = "Image too big! Please choose another one.";
//let wrongFileFormatMessage = "It is not a valid image file. Please choose another one.";
let missingFileErrorMessage = "Veuillez sélectionner une image.";
let tooBigFileErrorMessage = "L'image est trop volumineuse.";
let wrongFileFormatMessage = "Ce format d'image n'est pas accepté.";

let maxImageSize = 15000000;
var currentId = 0;

// Accepted file formats
let acceptedFileFormat = "image/jpeg,image/jpg,image/gif,image/png,image/bmp,image/webp,image/avif";

$(document).ready(() => {
    /* you can have more than one file uploader */
    $('.imageUploader').each(function () {
        let id = $(this).attr('id');
        let controlId = $(this).attr('controlId');
        let waitingImage = $(this).attr('waitingImage');
        let createMode = parseInt(id) === 0;

        let defaultImage = $(this).attr('imageSrc');
        $(this).append(`<img 
                         id="${controlId}_UploadedImage" 
                         name="${controlId}_UploadedImage" 
                         tabindex=0 class="UploadedImage"
                         src="${defaultImage}" 
                         waitingImage ="${waitingImage}">`);

        $(this).append(`<input 
                         id="${controlId}_ImageUploader" 
                         type="file" style="visibility:hidden;height:0px;"
                         accept="${acceptedFileFormat}">`);

        $(this).append(`<input 
                        style="visibility:hidden;height:0px;" 
                        class="fileUploadedExistRule fileUploadedSizeRule"
                        id="${controlId}" 
                        name="${controlId}" 
                        createMode = "${createMode}" 
                        waitingImage ="${waitingImage}">`);

        //$(this).append('<br><span>Cliquez et faites CTRL-V</span>');
        ImageUploader_AttachEvent(controlId);
        AddCustomValidator();
    });

    $(".UploadedImage").on('dragenter', function (e) {
        $(this).css('border', '2px solid blue');
    });

    $(".UploadedImage").on('dragover', function (e) {
        $(this).css('border', '2px solid blue');
        e.preventDefault();
    });

    $(".UploadedImage").on('dragleave', function (e) {
        $(this).css('border', '2px solid white');
        e.preventDefault();
    });

    $(".UploadedImage").on('drop', function (e) {
        var image = e.originalEvent.dataTransfer.files[0];
        $(this).css('background', '#D8F9D3');
        e.preventDefault();
        let id = $(this).attr('id').split('_')[0];
        let UploadedImage = document.querySelector('#' + id + '_UploadedImage');
        let waitingImage = UploadedImage.getAttribute("waitingImage");
        let ImageData = document.querySelector('#' + id);
        // store the previous uploaded image in case the file selection is aborted
        UploadedImage.setAttribute("previousImage", UploadedImage.src);

        // set the waiting image
        if (waitingImage !== "") UploadedImage.src = waitingImage;
        /* take some delai before starting uploading process in order to let browser to update UploadedImage new source affectation */
        let t2 = setTimeout(function () {
            if (UploadedImage !== null) {
                let len = image.name.length;

                if (len !== 0) {
                    let fname = image.name;
                    console.log(fname)
                    let ext = fname.split('.').pop().toLowerCase();

                    if (!validExtension(ext)) {
                        alert(wrongFileFormatMessage);
                        UploadedImage.src = UploadedImage.getAttribute("previousImage");
                    }
                    else {
                        let fReader = new FileReader();
                        fReader.readAsDataURL(image);
                        fReader.onloadend = () => {
                            UploadedImage.src = fReader.result;
                            ImageData.value = UploadedImage.src;
                        };
                    }
                }
                else {
                    UploadedImage.src = null;
                }
            }
        }, 30);
        $(this).css('border', '2px solid white');
        return true;
    });

});

function ImageUploader_AttachEvent(controlId) {
    // one click will be transmitted to #ImageUploader
    document.querySelector('#' + controlId + '_UploadedImage').
        addEventListener('click', () => {
            document.querySelector('#' + controlId + '_ImageUploader').click();
        });
    document.querySelector('#' + controlId + '_ImageUploader').
        addEventListener('change', preLoadImage);
}


function AddCustomValidator() {
    $.validator.addMethod("fileUploadedExistRule", function (value, element) { return CheckPhoto(element); }, missingFileErrorMessage);
    $.validator.addMethod("fileUploadedSizeRule", function (value, element) { return CheckPhotoSize(element); }, tooBigFileErrorMessage);
}

// Check if an image has been uploaded
function CheckPhoto(element) {
    let createMode = $(element).attr('createMode') === "true";
    if (createMode)
        return $(element).val() !== "";
    else
        return true;
}

// Check if uploaded image exceed maximum sixe
function CheckPhotoSize(element) {
    var files = $("#" + $(element).attr('id') + "_ImageUploader").get(0).files;
    if (files.length > 0)
        return files[0].size < maxImageSize;
    else
        return true;
}

function validExtension(ext) {
    return acceptedFileFormat.indexOf("/" + ext) > 0;
}

function preLoadImage(event) {
    // extract the id of the event target
    let id = event.target.id.split('_')[0];
    let UploadedImage = document.querySelector('#' + id + '_UploadedImage');
    let waitingImage = UploadedImage.getAttribute("waitingImage");
    let ImageUploader = document.querySelector('#' + id + '_ImageUploader');
    let ImageData = document.querySelector('#' + id);
    // store the previous uploaded image in case the file selection is aborted
    UploadedImage.setAttribute("previousImage", UploadedImage.src);
    // is there a file selection
    if (ImageUploader.value.length > 0) {

        // set the waiting image
        if (waitingImage !== "") UploadedImage.src = waitingImage;
        /* take some delai before starting uploading process in order to let browser to update UploadedImage new source affectation */
        let t2 = setTimeout(function () {
            if (UploadedImage !== null) {
                let len = ImageUploader.value.length;

                if (len !== 0) {
                    let fname = ImageUploader.value;
                    let ext = fname.split('.').pop().toLowerCase();

                    if (!validExtension(ext)) {
                        alert(wrongFileFormatMessage);
                        UploadedImage.src = UploadedImage.getAttribute("previousImage");
                    }
                    else {
                        let fReader = new FileReader();
                        console.log(ImageUploader.files[0])
                        fReader.readAsDataURL(ImageUploader.files[0]);
                        fReader.onloadend = () => {
                            UploadedImage.src = fReader.result;
                            ImageData.value = UploadedImage.src;
                        };
                    }
                }
                else {
                    UploadedImage.src = null;
                }
            }
        }, 30);
    }
    return true;
}

document.onpaste = function (event) {
    console.log(event.target)
    let id = event.target.id.split('_')[0];
    let UploadedImage = document.querySelector('#' + id + '_UploadedImage');
    let ImageData = document.querySelector('#' + id);
    let waitingImage = UploadedImage.getAttribute("waitingImage");
    if (waitingImage !== "") UploadedImage.src = waitingImage;
    // use event.originalEvent.clipboard for newer chrome versions
    var items = (event.clipboardData || event.originalEvent.clipboardData).items;
    // find pasted image among pasted items
    var blob = null;
    for (var i = 0; i < items.length; i++) {
        if (items[i].type.indexOf("image") === 0) {
            blob = items[i].getAsFile();
        }
    }
    // load image if there is a pasted image
    if (blob !== null) {
        var reader = new FileReader();
        reader.onload = function (event) {
            // console.log(event.target.result); // data url!
            UploadedImage.src = event.target.result;
            ImageData.value = UploadedImage.src;
        };
        reader.readAsDataURL(blob);
    }
}

//https://soshace.com/the-ultimate-guide-to-drag-and-drop-image-uploading-with-pure-javascript/