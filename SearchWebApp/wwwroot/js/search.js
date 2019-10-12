$(document).ready(function () {
  document.getElementById("searchStringInput").addEventListener("keydown",
    function (e) {
      if (e.keyCode === 13) {
        document.getElementById("submitButton").click();
      }
    });
});

function isInputNotEmpty(id) {
  if (document.getElementById(id).value === "") {
    return false;
  };
  return true;
}