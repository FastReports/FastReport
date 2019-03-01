<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <h2>Downloading</h2>
    <ul>
      <li>
        <a
          v-bind:href="backendPath + '1?format=png'"
          target="_blank"
        >
          Download Master-Detail report as PNG
        </a>
      </li>
      <li>
        <a
          v-bind:href="backendPath + '1?format=pdf'"
          target="_blank"
        >
          Download Master-Detail report as PDF
        </a>
      </li>
      <li>
        <a
          v-bind:href="backendPath + '1?format=html'"
          target="_blank"
        >
          Download Master-Detail report as HTML
        </a>
      </li>

      <br>
      <li>
        <a
          v-bind:href="backendPath + '2?format=png'"
          target="_blank"
        >
          Download Barcode report as PNG
        </a>
      </li>
      <li>
        <a
          v-bind:href="backendPath + '2?format=pdf'"
          target="_blank"
        >
          Download Barcode report as PDF
        </a>
      </li>
      <li>
        <a
          v-bind:href="backendPath + '2?format=html'"
          target="_blank"
        >
          Download Barcode report as HTML
        </a>
      </li>
    </ul>
    <h2>Inline reports</h2>
    <input type="radio" id="master" value="1?format=html&inline=true" v-model="selectedReport">
    <label for="one">Master-Detail</label>
    <br>
    <input type="radio" id="bar" value="2?format=html&inline=true" v-model="selectedReport">
    <label for="one">Barcodes</label>
    <br>  
    <iframe v-bind:src="backendPath + selectedReport"
    width="800" height="600"/>
  </div>
</template>

<script>
export default {
  name: "Reports",
  data() {
    return {
      msg: "FastReport.Net Core + Vue.js Demo",
      backendPath: "http://localhost:53300/api/reports/",
      selectedReport: "1?format=html&inline=true"
    };
  }
};
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
