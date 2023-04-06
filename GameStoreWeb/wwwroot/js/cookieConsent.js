$(document).ready(function () {
    if (!getCookie('cookie_consent')) {
        Swal.fire({
            title: 'Cookie Consent',
            text: 'This website uses cookies to ensure you get the best experience. Do you accept the use of cookies?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Accept',
            cancelButtonText: 'Reject',
        }).then((result) => {
            if (result.value) {
                setCookie('cookie_consent', 'true', 30);
                Swal.fire('Thank you!', 'You have accepted cookies.', 'success');
            } else {
                Swal.fire('Notice', 'This website uses cookies to provide some features. You may experience limited functionality.', 'info');
            }
        });
    }
});

// Set cookie
function setCookie(name, value, days) {
    var date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    var expires = 'expires=' + date.toUTCString();
    document.cookie = name + '=' + value + ';' + expires + ';path=/';
}

// Get cookie
function getCookie(name) {
    var cookieName = name + '=';
    var decodedCookie = decodeURIComponent(document.cookie);
    var cookieArray = decodedCookie.split(';');
    for (var i = 0; i < cookieArray.length; i++) {
        var cookie = cookieArray[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(cookieName) === 0) {
            return cookie.substring(cookieName.length, cookie.length);
        }
    }
    return '';
}