
function onOverlayClick() {
    var dialog = document.getElementById("dialog").ej2_instances[0];
    dialog.hide();
};

function onAddEstimateButtonClick() {
    debugger;
    var dialog = document.getElementById("dialog").ej2_instances[0];
    dialog.header = "Add Estimate";
    dialog.content = "New client?";
    dialog.show();
};

function ondlgYesButtonClick() {
    window.location.href = "/Customer/Create";
};

function ondlgNoButtonClick() {
    window.location.href = "/Customer";
};


function onLoadFunctions() {
    
    $.ajax({
        url: '/DashboardPanel/GetOpenedWorkOrders',
        type: 'Post',
        dataType: 'json',        
        success: function (workOrders) {
            if (workOrders != 0) {
                document.getElementById("activeWorkOrders").innerHTML = workOrders;
            }
        },
        error: function (ex) {
            console.log(ex); 
        }
    });

    loadProfile();
}

function loadProfile() {
    debugger;

    var host = window.location.host;    
    var url = 'https://' + host + '/Account/GetProfilePicturePath';

    $.ajax({
        url: url,
        type: 'Post',
        dataType: 'json',
        success: function (user) {
            console.log(user);

            debugger;
            if (user.profilePictureAltPath != null) {
                document.getElementById('userProfilePicture').src = user.profilePictureAltPath;             
            }
            else {
                document.getElementById("userProfilePicture").src = "/images/blankProfilePicture.png";
            }

            document.getElementById("userFullName").innerHTML=user.fullName;
        },
        error: function (ex) {
            console.log(ex);
        }
    });
}

function getBackgroundProfilePicture() {
    
    $.ajax({
        url: '/Account/GetProfilePicturePath',
        type: 'Post',
        dataType: 'json',
        success: function (user) {
            console.log(user);

            debugger;
            if (user.profilePictureAltPath != null) {
                document.getElementById('profilePictureBackground').style.background = 'url('+user.profilePictureAltPath+')';
            }
            //else {
            //    document.getElementById("profilePictureBackground").style.background = 'url('+/images/blankProfilePicture.png+')';
            //}            
        },
        error: function (ex) {
            console.log(ex);
        }
    });
}

function changeProfilePicture(inputId) {
    debugger;

    var input = document.getElementById(inputId).files[0];      
    var formData = new FormData();
    formData.append('file', input); 

    $.ajax({
        url: '/Account/ChangeProfilePic?handler=file',
        type: 'Post',
        data: formData,
        cache: false,
        processData: false,
        contentType: false,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (response) {            

            window.location.reload(true);           

        },
        error: function (ex) {
            console.log(ex);
        }
    });
}

//function openModal(inputId) {
//    debugger;

//    var input = document.getElementById(inputId).files[0];
//    var path = URL.createObjectURL(input);  

//    $('#exampleModalCenter').modal('show');

//    //initialize Croppie
//    var basic = $('#main-cropper').croppie
//        ({
//            viewport: { width: 300, height: 300 },
//            boundary: { width: 400, height: 400 },
//            showZoomer: true,          
//        });

//    //basic.bind(input).then(function () {
//    //    basic.result('file', { type: 'blob' }).then(function (blob) {
//    //        console.log(blob);

//    //    })
//    //})

//    $('#main-cropper').croppie('bind', {
//        url: path
//    });

//    //$('#exampleModalCenter').on('show.bs.modal', function () {

//    //});

//    $('#btnUpdate').on('click', function () {
//        debugger;
//        var formData = new FormData();
//        var blobresult;

//        var result = basic.croppie('result', { type: 'blob' }).then(function (blob) {
//            console.log(blob);  
//            blobresult = blob;
//            formData.append('file', blob);
//        });
//       /* var result = $('#main-cropper').croppie('result', { type: 'blob'});*/
//        console.log(result);  
//        console.log(blobresult);
//        //for (var value of formData.values()) {
//        //    console.log(value);
//        //}


//        $.ajax({
//            url: '/Account/ChangeProfilePic?handler=file',
//            type: 'Post',
//            data: formData,
//            cache: false,
//            processData: false,
//            contentType: false,
//            beforeSend: function (xhr) {
//                xhr.setRequestHeader("XSRF-TOKEN",
//                    $('input:hidden[name="__RequestVerificationToken"]').val());
//            },
//            success: function (response) {

//                window.location.reload(true);

//            },
//            error: function (ex) {
//                console.log(ex);
//            }
//        });
//    });
//}

