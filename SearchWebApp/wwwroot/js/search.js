$(document).ready(function () {
  document.getElementById("searchStringInput").addEventListener("keydown",
    function (event) {
      if (event.keyCode === 13) {
        event.preventDefault();
        document.getElementById("submitButton").click();
      }
    });
});

function isInputElementNotEmpty(id) {
  if (document.getElementById(id).value === "") {
    return false;
  };
  return true;
}