// = null permits the parameter to be optional
function loadView(status, userData = null, actionMethod = 'GET', accNum = null)
{
    var apiUrl = "view/login/defaultview";

    if (status === "authview") {
        apiUrl = "view/login/authview";
    } else if (status === "error") {
        apiUrl = "view/login/errorview";
    } else if (status === "createAccount") {
        apiUrl = "user/UserFunctions/createAccount";
    } else if (status === "getAccount") {
        apiUrl = "user/UserFunctions/getAccount";
    } else if (status === "getAllTransactions") {
        apiUrl = "user/UserFunctions/auditAll";
    } else if (status === "getAcctTransactions") {
        apiUrl = "user/UserFunctions/auditAccount";
    } else if (status === "modifyUser") {
        apiUrl = "user/UserFunctions/modifyUser";
    } else if (status === "createUser") {
        apiUrl = "user/UserFunctions/createUser";
    } else if (status === "addNewUser") {
        apiUrl = "user/UserFunctions/addNewUser";
    } else if (status === "sendModifyRequest") {
        apiUrl = "user/UserFunctions/sendModifyRequest";
    } else if (status === "getAccountTransactionsOrdered") {
        apiUrl = "user/UserFunctions/auditAccountOrdered";
    } else if (status === "addNewAccount") {
        apiUrl = "user/UserFunctions/addNewAccount";
    } else if (status === "transfer") {
        apiUrl = "user/UserFunctions/Transfer";
    } else if (status === "manageUsers") {
        apiUrl = "admin/AdminFunctions/manageUsers";
    } else if (status === "displayUsers") {
        apiUrl = "admin/AdminFunctions/displayUsers";
    }

    console.log('User data: ', userData);
    console.log(`API URL: ${apiUrl}`)
    // Define the request options
    const requestOptions = {
        method: actionMethod,
        headers: {
            'Content-Type': 'application/json'
        }
    };

    // If using POST and userData is not null, add the body to the request
    if (actionMethod === 'POST' && userData) {
        let requestBody; // Declare requestBody variable that can be reassigned
        if (status === "getAccount" || status === "getAcctTransactions" || status === "getAccountTransactionsOrdered") {
            // Construct UserRequest object
            console.log("Account Number: " + accNum);
            requestBody = {
                user: userData,
                accountNumber: accNum || null
            };
        }
        else // sendModifyRequest has been structued as UserRequest in Index.cshtml
        {
            // Pass UserDataIntermed object directly
            requestBody = userData; 
        }
        console.log('Data passed: ', requestBody);
        requestOptions.body = JSON.stringify(requestBody);
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


// pUsername is the username entered by the user, pPassword is the password entered by the user, and pAction is the action to perform
function performAuthParameters(pUsername, pPassword, pAccNum, pAction) {
    var data = {
        user: {
            username: pUsername,
            password: pPassword
        },
        accountNumber: pAccNum === 0 ? null : pAccNum // Convert 0 passed in to null
    };
    console.log("Action: " + pAction);
    const apiUrl = 'user/UserFunctions/auth'

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
                loadView(pAction, data.user, 'POST', pAccNum); // pAction loads the correct view with the user data and the POST method
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