//tab ticket

var i, tabcontent, tablinks;
tabcontent = document.getElementsByClassName("tabcontent");
tabcontent2 = document.getElementsByClassName("tabcontent2");
function ticket(evt, ticketName) {
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" lete-active", "");
    }
    document.getElementById(ticketName).style.display = "block";
    evt.currentTarget.className += " lete-active";
 
}

//tab upproduct
function openTabUpProduct(evt, cityName) {
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";
}

//tab RECHANGEmONEY
function tabRechangeMoney(evt, cityName) {
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" lete-active", " ");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " lete-active";
}

//tab gui link bao gia

function tabSendLink(evt, ticketName) {
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" lete-active", "");
    }
    document.getElementById(ticketName).style.display = "block";
    evt.currentTarget.className += " lete-active";
}



//tab thong ke don hang
function tabStaticOrder(evt, cityName) { 
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " active";
}

//tab don hang luu kho
function createSaveStorage(evt, cityName) {
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" lete-active", "");
    }
    document.getElementById(cityName).style.display = "block";
    evt.currentTarget.className += " lete-active";
}


//tab FAQ
function tabFAQ(evt, faqName) {
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active-faq", "");
    }
    document.getElementById(faqName).style.display = "block";
    evt.currentTarget.className += " active-faq";

    // Get the element with id="defaultOpen" and click on it

}

//tab PHI DICH VU
function tabServiceCharge(evt, scName) {
    for (i = 0; i < tabcontent2.length; i++) {
        tabcontent2[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tab-margin");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    document.getElementById(scName).style.display = "block";
    evt.currentTarget.className += " active";

    // Get the element with id="defaultOpen" and click on it

}
// Get the element with id="defaultOpen" and click on it
document.getElementById("defaultOpen").click();

