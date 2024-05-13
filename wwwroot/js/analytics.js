function setMatomoCookie(cvalue) {
    const d = new Date();
    const days = 365;
    d.setTime(d.getTime() + (days * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = "cookie_policy=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function checkMatomoCookie() {
    let analytics = getCookie("cookie_policy");
    let banner = document.getElementById("analyticsBanner");

    if (analytics == "enabled" || analytics == "disabled") {
        banner.style.display = "none";
    } else {
        banner.style.display = "block";
    }
}

function acceptAnalytics() {
    setMatomoCookie("enabled");
    //checkMatomoCookie();

    let elem = document.getElementById("acceptedCookies")
    elem.removeAttribute("hidden");

    let contentElem = document.getElementById("analyticsContent")
    contentElem.setAttribute("hidden", "");
}

function rejectAnalytics() {
    setMatomoCookie("disabled");
    //checkMatomoCookie();

    let elem = document.getElementById("rejectedCookies")
    elem.removeAttribute("hidden");

    let contentElem = document.getElementById("analyticsContent")
    contentElem.setAttribute("hidden", "");
}

function analyticsEnabled() {
    let analytics = getCookie("cookie_policy");

    return analytics == "enabled";
}

function hideAnalytics() {
    let elem = document.getElementById("analyticsBanner")
    elem.style.display = "none";
}