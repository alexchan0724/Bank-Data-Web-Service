function loadView(status, userData = null, actionMethod = 'GET') {
    var apiUrl = "view/login/defaultview";

    if (status === "authview") {
        apiUrl = "view/login/authview";
    } else if (status === "error") {
        apiUrl = "view/login/errorview";
    }

    console.log(`Loading view: ${status} with method: ${actionMethod}`);

    // Define the request options
    const requestOptions = {
        method: actionMethod,
        headers: {
            'Content-Type': 'application/json'
        }
    };

    // If using POST and userData is not null, add the body to the request
    if (actionMethod === 'POST' && userData) {
        console.log('User data:', userData);
        requestOptions.body = JSON.stringify(userData);
    }

    fetch(apiUrl, requestOptions) // Allows POST and GET methods
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
            console.error(error);
        });
}


function performAuth() {
    var name = document.getElementById('SName').value;
    var password = document.getElementById('SPass').value;
    var data = {
        username: name,
        password: password
    };
    const apiUrl = 'view/login/auth'

    const headers = {
        'Content-Type': 'application/json', // Specify the content type as JSON if you're sending JSON data
        // Add any other headers you need here
    };

    const requestOptions = {
        method: 'POST',
        headers: headers,
        body: JSON.stringify(data) // Convert the data object to a JSON string
    };

    fetch(apiUrl, requestOptions)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            // Handle the data from the API
            if (data.login) {
                const userData = data.user;
                loadView("authview", userData, 'POST');
                //document.getElementById('LogoutButton').style.display = "block";
            }
            else {
                loadView("error");
            }

        })
        .catch(error => {
            // Handle any errors that occurred during the fetch
            console.error('Fetch error:', error);
        });
}
