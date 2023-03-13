<template>
    <div class="hello">
        <h1>{{ msg }}</h1>
        <h1>OpenSource Demo</h1>
        <h2>Downloading</h2>
    <ul>
        <li>
            <a v-bind:href="backendPath + 'download?name=Master-Detail&format=png'"
               target="_blank">
                Download Master-Detail report as PNG
            </a>
        </li>
        <li>
            <a v-bind:href="backendPath + 'download?name=Master-Detail&format=html'"
               target="_blank">
                Download Master-Detail report as HTML
            </a>
        </li>

        <br>
        
        <li>
            <a v-bind:href="backendPath + 'download?name=Barcode&format=png'"
               target="_blank">
                Download Barcode report as PNG
            </a>
        </li>
        <li>
            <a v-bind:href="backendPath + 'download?name=Barcode&format=html'"
               target="_blank">
                Download Barcode report as HTML
            </a>
        </li>
    </ul>
        <h2>Inline reports</h2>
        <input type="radio" id="master" value="showreport?name=Master-Detail" v-model="selectedReport">
        <label for="one">Master-Detail</label>
        <br>
        <input type="radio" id="bar" value="showreport?name=Barcode" v-model="selectedReport">
        <label for="one">Barcodes</label>
        <br>

        <iframe id="iframe" :src=backendPath+selectedReport
                width="1025" height="1200" />
    </div>
    <footer>
        <p><a href="https://www.fast-report.com">&copy; 2023 - Fast Reports, Inc.</a></p>
    </footer>
</template>

<script>
    export default {
        name: "ReportsBase",
        htmlrender: "",
        data() {
            return {
                msg: "FastReport.Net Core + Vue.js", 
                backendPath: "https://localhost:5001/api/reports/",
                selectedReport: "showreport?name=Barcode",
                html: ''
            };
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h1,
h2 {
  font-weight: normal;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
