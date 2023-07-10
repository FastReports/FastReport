using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace FastReport.Web
{
    partial class WebReport
    {
        private const string validation = @"
    var pageSelector = document.getElementById('PageSelector');
    var pageRange = pageSelector ? pageSelector.value : '';
    var validationRegex = new RegExp(""^(\\s*\\d+\\s*\\-\\s*\\d+\\s*,?|\\s*\\d+\\s*,?)+$"");

    if(!validationRegex.test(pageRange) && pageRange !== """"){
        return;
    }";

        private const string template_pscustom =
@"    const pageRange = input.value.trim();
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

      if (validationRegex.test(pageRange) || pageRange === """") {
        PageSelector = `&PageRange=PageNumbers&PageNumbers=${PageSelectorInput[0].value}`;
        OnFirst[0].classList.remove('activeButton');
        OnAll[0].classList.remove('activeButton');
        input.classList.remove('input-error');
        okButton.classList.remove(""fr-webreport-popup-disabled-button"");
        okButton.disabled = isError;
      } else {
        input.classList.add('input-error');
        okButton.classList.add(""fr-webreport-popup-disabled-button"");
        okButton.disabled = true;
  }
";

        private const string template_modalcontainerscript =
@" var but = document.querySelectorAll('.fr-webreport-settings-btn');
   var modalOverlay = document.querySelector('.modalcontainer-overlay');
   var modalBtnsSubmit = document.querySelectorAll('.fr-webreport-popup-content-btn-submit');
   var activebtn = document.querySelectorAll('.fr-webreport-popup-content-export-parameters-button');          
   var modals = document.querySelectorAll('.modalcontainer');
   var okButton = document.getElementById('okButton');
   var input = document.getElementById('PageSelector');

        modalBtnsSubmit.forEach((el) => {
    el.addEventListener('click', (e) => {
        modalOverlay.classList.remove('modalcontainer-overlay--visible');
	    modals.forEach((el) => {
		    el.classList.remove('modalcontainer--visible');
	    });
    });
    });

    but.forEach((el) => {
    el.addEventListener('click', (e) => {
    let path = e.currentTarget.getAttribute('data-path');
   
    modals.forEach((el) => {
        el.classList.remove('modalcontainer--visible');
    });
        });
    });

    modalOverlay.addEventListener('click', (e) => {
      
    if (e.target == modalOverlay || e.target == modalBtnsSubmit) {
	    modalOverlay.classList.remove('modalcontainer-overlay--visible');
	    modals.forEach((el) => {
            el.innerHTML = '';
		    el.classList.remove('modalcontainer--visible');
	    });
      }
    });

    activebtn.forEach((el) => {
        el.addEventListener('click', (e) => {
            {
                if(el.getAttribute('name') != 'OnRgbClick' && el.getAttribute('name') != 'OnCmykClick'){{
                    el.classList.toggle('activeButton');
                }}
            }
        });
    });

    var PageSelector = '&PageRange=All';
    var OnAll = document.getElementsByName('OnAllClick');
    var OnFirst = document.getElementsByName('OnFirstClick');
    var PageSelectorInput = document.getElementsByName('PageSelectorInput');

    function OnFirstClick() {
        for (var i = 0; i < PageSelectorInput.length; i++) {
            PageSelectorInput[i].value = CurrentPage.value;
        }
        for (var i = 0; i < OnAll.length; i++) {
            OnAll[i].classList.remove('activeButton');
        }
        PageSelectorInput[0].value = CurrentPage.value;
        OnAll[0].classList.remove('activeButton');
        PageSelector = '&PageRange=PageNumbers&PageNumbers=' + CurrentPage.value;
        okButton.disabled = false;
        okButton.classList.remove(""fr-webreport-popup-disabled-button"");
        input.classList.remove('input-error');
    }

    function OnAllClick() {
        PageSelectorInput[0].value = '1 - ' + AllPages.value;
        OnFirst[0].classList.remove('activeButton');
        PageSelector = '&PageRange=All';
        okButton.disabled = false;
        okButton.classList.remove(""fr-webreport-popup-disabled-button"");
        input.classList.remove('input-error');
    }
    var PSFirst = '';
    var PSLast = ''
    var CustomPagesArray";

        string template_modalcontainer()
        {
            var templateModalContainer = $@"
<div class=""modalcontainers"">
    <div class=""modalcontainer-overlay"">
        <script>
            setTimeout(function () {{
                {template_FR}.getExportSettings();
            }}, 100);  
        </script>
        <div class = ""content-modalcontainer""></div>
    </div>
</div>";
            return templateModalContainer;
        }
    }

}
