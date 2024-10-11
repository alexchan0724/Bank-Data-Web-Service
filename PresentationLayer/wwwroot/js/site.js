function loadView(status) {
    var apiUrl = "view/login/defaultview"

    console.log("Loading view: " + status);
    fetch(apiUrl)
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok");
            }
            return response.text();
        })
        .then(data => {
            document.getElementById("main").innerHTML = data;
            console.log('Data:', data);
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

function performAuth() {
    
}
