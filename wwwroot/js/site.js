//Code adapted from GeeksforGeeks(2023)
function snackbarFunc() {
    // Get the snackbar DIV
    var x = document.getElementById("snackbar");
    // Add the "show" class to DIV
    x.className = "show";
    // After 3 seconds, remove the show class from DIV
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}
//End of code adapted


// Code adapted from W3Schools(n.d.-c)
function openTab(tabName) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent"); //store div class in tabcontent variable
    for (i = 0; i < tabcontent.length; i++) { //iterate through div classes and set to not display
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks"); //get button class div name
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", ""); //get id of tab that needs to be displated and set as active
    }
    document.getElementById(tabName).style.display = "block";  //display tabName as block
    event.currentTarget.className += " active"; //set class as active
}

document.getElementById("defaultTab").click(); //display default tab

// End of code adapted