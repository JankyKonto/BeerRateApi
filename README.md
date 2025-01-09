# How to launch the app

1. Install Visual Studio 2022 from [https://visualstudio.microsoft.com/pl/vs/]().
1. Install Git from [https://git-scm.com/downloads]().
1. Clone this repository by running `git clone https://github.com/JankyKonto/BeerRateApi.git` command.
1. Run the `BeerRateApi.sln` file using Visual Studio 2022.
1. In `appsettings.json` file you have to adjust the following things:
     ```json
       "ConnectionStrings": {
      "DefaultConnection": "{{connection_string_to_database}}"
    },
    "EmailSenderSettings": {
      "Host": "{{your_email_host}}",
      "HostAddress": "{{your_email_address}}"
    },
    "ClientAppSettings": {
        "Address": "{{frontend_app_address}}"
    }
    ```
1. You have to set up an environmental variable called `GOOGLE_APP_PASSWORD` with your email account password.
1. Start the app by clicking on the green arrow in the top bar of the app.
