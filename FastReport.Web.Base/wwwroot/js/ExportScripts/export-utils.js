'use strict';
export function IsValid() {
    var pageSelector = document.getElementById('PageSelector');
    var pageRange = pageSelector ? pageSelector.value : '';
    var validationRegex = new RegExp("^(\\s*\\d+\\s*\\-\\s*\\d+\\s*,?|\\s*\\d+\\s*,?)+$");

    if (!validationRegex.test(pageRange) && pageRange !== "") {
        return false;
    }
    return true;
}

export function onChange() {
    var okButton = document.getElementById('okButton');
    var input = document.getElementById('PageSelector');

    const pageRange = input.value.trim();
    const validationRegex = /^(\s*\d+\s*-\s*\d+\s*,?|\s*\d+\s*,?)+$/;
    let isError = false;
    let numbers = [];

    if (pageRange) {
        numbers = pageRange.match(/(-\d+|\d+)(,\d+)*(\.\d+)*/g) || [];
        numbers = numbers.map(n => Number(n.replace(/,/g, '')));
        numbers.forEach(elem => {
            if (elem > AllPages.value) {
                isError = true;
            }
        });
    }

    if (validationRegex.test(pageRange) || pageRange === "") {
        window.frActions.PageSelector = `&PageRange=PageNumbers&PageNumbers=${pageRange}`;
        input.classList.remove('input-error');
        okButton.classList.remove("fr-popup-disabled-button");
        okButton.disabled = isError;
    } else {
        input.classList.add('input-error');
        okButton.classList.add("fr-popup-disabled-button");
        okButton.disabled = true;
    }
}
var modalOverlay;
var modalBtnsSubmit;
var modals;
export function createLisners() {
    var but = document.querySelectorAll('.fr-settings-btn');
    var activebtn = document.querySelectorAll('.fr-popup-content-export-parameters-button');
    modals = document.querySelectorAll('.modalcontainer');
    modalOverlay = document.querySelector('.modalcontainer-overlay');
    modalBtnsSubmit = document.querySelectorAll('.fr-popup-content-btn-submit');
    window.frActions.PageSelector = '&PageRange=All';

    modalBtnsSubmit.forEach((el) => {
        el.addEventListener('click', (e) => {
            modalOverlay.classList.remove('modalcontainer-overlay--visible');
            modals.forEach((el) => {
                el.classList.remove('modalcontainer--visible');
            });
        });
    });

    but.forEach((el) => {
        el.removeEventListener('click', RadioButtonStateChange);
        el.addEventListener('click', RadioButtonStateChange);
    });

    modalOverlay.addEventListener('click', ModalOverlayClick);

    activebtn.forEach((el) => {
        el.addEventListener('click', (e) => {
            {
                if (el.getAttribute('name') != 'OnRgbClick' && el.getAttribute('name') != 'OnCmykClick' &&
                    el.getAttribute('name') != 'OnFirstClick' && el.getAttribute('name') != 'OnAllClick') {
                    {
                        el.classList.toggle('activeButton');
                    }
                }
            }
        });
    });

    AddAction('OnFirstClick', OnFirstClick);
    AddAction('OnAllClick', OnAllClick);
    AddAction('OnPageSelectorChange', onChange);
}

function ModalOverlayClick(e) {
    if (e.target == modalOverlay || e.target == modalBtnsSubmit) {
        modalOverlay.classList.remove('modalcontainer-overlay--visible');
        modals.forEach((el) => {
            el.innerHTML = '';
            el.classList.remove('modalcontainer--visible');
        });
    }
}
function RadioButtonStateChange(e) {
    let path = e.currentTarget.getAttribute('data-path');

    modals.forEach((el) => {
        el.classList.remove('modalcontainer--visible');
    });
}

export function OnFirstClick() {
    var OnFirst = document.getElementsByName('OnFirstClick');
    var OnAll = document.getElementsByName('OnAllClick');
    var PageSelectorInput = document.getElementsByName('PageSelectorInput');
    var okButton = document.getElementById('okButton');
    var input = document.getElementById('PageSelector');
    for (var i = 0; i < PageSelectorInput.length; i++) {
        PageSelectorInput[i].value = CurrentPage.value;
    }
    for (var i = 0; i < OnAll.length; i++) {
        OnAll[i].classList.remove('activeButton');
    }
    OnFirst[0].classList.add('activeButton');
    PageSelectorInput[0].value = CurrentPage.value;
    OnAll[0].classList.remove('activeButton');
    window.frActions.PageSelector = '&PageRange=PageNumbers&PageNumbers=' + CurrentPage.value;
    okButton.disabled = false;
    okButton.classList.remove("fr-popup-disabled-button");
    input.classList.remove('input-error');
}

export function OnAllClick() {
    var OnAll = document.getElementsByName('OnAllClick');
    var OnFirst = document.getElementsByName('OnFirstClick');
    var PageSelectorInput = document.getElementsByName('PageSelectorInput');
    var okButton = document.getElementById('okButton');
    var input = document.getElementById('PageSelector');
    OnAll[0].classList.add('activeButton');
    PageSelectorInput[0].value = '1 - ' + AllPages.value;
    OnFirst[0].classList.remove('activeButton');
    window.frActions.PageSelector = '&PageRange=All';
    okButton.disabled = false;
    okButton.classList.remove("fr-popup-disabled-button");
    input.classList.remove('input-error');
}

export function AddAction(name, action) {
    if (!window.frActions)
        window.frActions = {};
    window.frActions[name] = action;
}

