let accordionTitles = document.querySelectorAll(".accordionTitle");
let queryFiltersForm = document.querySelector('#qualificationsFilterForm')

//the tags in the selected filters panel
let selectedFilters = document.getElementsByClassName("removeFilter");

//the tags for integer values (GLH and TQT) in the selected filters panel
let selectedIntFilters = document.getElementsByClassName("removeIntFilter");

//filters where you need to click out to apply the filter (TQT & GLH)
let focusOutElements = document.getElementsByClassName("focusOutFilters");

//the form identifier so we can keep a track of the selected quals
let selectedQualsForm = document.querySelector('#compareQualifications');

//the set to make sure no qual is added twice
let selectedQuals = new Set();

//to keep a track of how many quals are selected for the selected quals stats on page
let numSelected = 0;

//init document
document.addEventListener("DOMContentLoaded", function (event) {
    disableCompareBtn();
    updateSelectedStats();
    checkMatomoCookie();
});

//open and close the filter panels
accordionTitles.forEach(accordionTitle => {
    accordionTitle.addEventListener("click", () => {
        accordionTitle.classList.toggle("is-open");
        accordionTitle.parentElement.classList.toggle("is-open")
    })
})

//whenever a filter checkbox is changed, 
queryFiltersForm.querySelectorAll('input[type="checkbox"]').forEach(i => {
    i.onchange = () => { queryFiltersForm.submit() }
});

//when a tag in the filters panel is clicked, remove the filter and submit the form
for (let i = 0; i < selectedFilters.length; i++) {
    selectedFilters[i].addEventListener('click', function () {
        //data-checkbox represents the actual filter checkbox in the filters form
        let cBox = document.getElementById(this.getAttribute("data-checkbox"));

        if (cBox.checked) {
            cBox.checked = false;
        } else {
            cBox.checked = true;
        }
        queryFiltersForm.submit()
    }, false);
}

//when the glh or tqt tags in the filters panel are clicked, remove the filter and submit the form
for (let i = 0; i < selectedIntFilters.length; i++) {
    selectedIntFilters[i].addEventListener('click', function () {
        //data-box represents the actual filter box in the filters form
        let box = document.getElementById(this.getAttribute("data-box"));
        box.value = "";
        queryFiltersForm.submit()
    }, false);
}

//when the glh or tqt filters are focused out, submit the form
for (let i = 0; i < focusOutElements.length; i++) {
    focusOutElements[i].addEventListener('focusout', function () {
        queryFiltersForm.submit()
    }, false);
}

//check if session storage is set and update the stats
if (sessionStorage.getItem("selectedQuals")) {
    selectedQualsArr = JSON.parse(sessionStorage.getItem("selectedQuals"));
    selectedQuals = new Set([...selectedQualsArr]);

    //check the session stored qual checkboxes
    selectedQualsArr.forEach((i) => {
        let cbx = document.getElementById("qualification-" + i);
        if (cbx) {
            cbx.checked = true;
        }
    });

    numSelected = selectedQualsArr.length;
    updateSelectedStats();
}

//update the session storage when quals are selected / unselected
selectedQualsForm.querySelectorAll('input[type="checkbox"]').forEach(i => {
    i.onchange = () => {
        if (i.checked) {
            let qualsArr = [...selectedQuals];
            qualsArr.splice(0, 0, i.value);
            selectedQuals = new Set([...qualsArr]);
            numSelected++;
        } else {
            selectedQuals.delete(i.value);
            numSelected--;
        }

        sessionStorage.setItem("selectedQuals", JSON.stringify([...selectedQuals]));

        disableCompareBtn();
        updateSelectedStats();
    }
});

//when the compare / download CSV form is updated, set the form's selectedQuals property with the latest list of selected quals 
function appendSelectedQuals() {
    let selectedCompareQuals = document.getElementById("selectedQuals");

    selectedCompareQuals.value = JSON.parse(sessionStorage.getItem("selectedQuals"));
}

//disables/enables the compare and csv btns based on how many quals are selected
function disableCompareBtn() {
    let compareBtn = document.getElementById("compareButton");
    //let csvBtn = document.getElementById("csvButton");

    if (selectedQuals.size < 2) {
        compareBtn.setAttribute("disabled", "true");
        compareBtn.setAttribute("aria-disabled", "true");
    }
    else {
        compareBtn.removeAttribute("disabled");
        compareBtn.removeAttribute("aria-disabled");
    }
}

function clearSelection() {
    selectedQuals = new Set();
    sessionStorage.setItem("selectedQuals", JSON.stringify([...selectedQuals]));

    let qualCbxs = document.getElementsByName("qualificationNumbers");

    qualCbxs.forEach((i) => {
        i.checked = false;
    });

    numSelected = 0;

    updateSelectedStats();
}

function updateSelectedStats() {
    let selectedInfo = document.getElementById("numSelected");
    let selectedInfoElem = document.getElementById("selectedStats");

    if (numSelected > 0) {
        selectedInfo.innerText = numSelected == 1 ? "1 Qualification selected." : numSelected + " Qualifications selected.";
        selectedInfoElem.style.display = "block";
    } else {
        selectedInfoElem.style.display = "none";
        disableCompareBtn();
        selectedInfo.innerText = "0 Qualifications selected.";
    }

}