//function showToast(message, type) {
//    var toaster = document.getElementById('toaster');
//    toaster.innerText = message;
//    toaster.className = 'toaster ' + type;
//    toaster.style.display = 'block';
//    setTimeout(function () {
//        toaster.style.display = 'none';
//    }, 3000);
//}

//document.addEventListener('DOMContentLoaded', function () {
//    var successMessage = '@TempData["SuccessMessage"]';
//    var failureMessage = '@TempData["ErrorMessage"]';

//    if (successMessage) {
//        showToast(successMessage, 'success');
//    }

//    if (failureMessage) {
//        showToast(failureMessage, 'failure');
//    }
//});