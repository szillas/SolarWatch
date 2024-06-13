# SolarWatch
SolarWatch is a full-stack web application project that provides sunrise and sunset times for a given city on a given date in UTC Time. The project is not yet finished. The backend is an ASP.NET Web API that is connected to a Microsoft SQL database. The frontend uses React with the help of CSS styling.

<a name="readme-top"></a>

<!-- Header -->
<div style="text-align: center;">

<h3 style="text-align: center;">SolarWatch</h3>

  <p style="text-align: center;">
    SolarWatch is a full-stack web application project that provides sunrise and sunset times for a given city on a given date in UTC Time.
    <br />
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>

### Built With

* [![React][React.js]][React-url]
* [![dotnet][dotnet.com]][dotnet-url]
* [![css][css.org]][css-url]
* [![MSSQL][mssql.com]][mssql-url]

<p style="text-align: right;">(<a href="#readme-top">back to top</a>)</p>


<!-- GETTING STARTED -->
## Getting Started

This section helps to set up the project locally and gives instructions about how to run it.

### Prerequisites  

The project uses OpenWeather's API. The API proides 1000 API calls for free, but for every call we need an API key. 
Get a free API key at https://openweathermap.org/ by registering.

#### Setup and run with Docker

1. Clone the repo
   ```sh
   git clone https://github.com/szillas/SolarWatch.git
   ```

2. Inside the main folder there is a sample.env file. Rename it to just .env .
   ```sh
   smple.env  ->  .env
   ```  

3. Inside the .env file, there is an SQLSERVER_PASSWORD and a SOLARWATCH__SECRETKEY environment variable. The program can run with these default values, but I would advise
to use your own password and secretkey, that should be strong as the default values show.  

4. Inside the .env file, there is one more environment variable, called SOLARWATCH__OPENWEATHERMAPKEY. Change it to your own API key from [OpenWeather.](https://openweathermap.org/)  

5. Build and run the app with docker-compose. Start the application from the project directory:
   ```sh
   docker compose up
   ```  

6. The building process and starting the database needs some time, but after the container runs, and the mssql server starts, you can reach the frontend on http://localhost:8082/


#### Setup and run without Docker

1. Clone the repo
   ```sh
   git clone https://github.com/szillas/SolarWatch.git
   ```

2. 





















<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[React.js]: https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB
[React-url]: https://reactjs.org/
[dotnet.com]: https://img.shields.io/badge/ASP.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=23512BD4
[dotnet-url]: https://dotnet.microsoft.com/en-us/apps/aspnet
[css.org]: https://img.shields.io/badge/css3-black?style=for-the-badge&logo=css3&logoColor=1572B6
[css-url]: https://developer.mozilla.org/en-US/docs/Web/CSS
[mssql.com]: https://img.shields.io/badge/Microsoft%20SQL%20Server-blue?style=for-the-badge&logoColor=23512BD4
[mssql-url]: https://www.microsoft.com/en-us/sql-server


[contributors-shield]: https://img.shields.io/github/contributors/github_username/repo_name.svg?style=for-the-badge
[contributors-url]: https://github.com/github_username/repo_name/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/github_username/repo_name.svg?style=for-the-badge
[forks-url]: https://github.com/github_username/repo_name/network/members
[stars-shield]: https://img.shields.io/github/stars/github_username/repo_name.svg?style=for-the-badge
[stars-url]: https://github.com/github_username/repo_name/stargazers
[issues-shield]: https://img.shields.io/github/issues/github_username/repo_name.svg?style=for-the-badge
[issues-url]: https://github.com/github_username/repo_name/issues
[license-shield]: https://img.shields.io/github/license/github_username/repo_name.svg?style=for-the-badge
[license-url]: https://github.com/github_username/repo_name/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/linkedin_username
[product-screenshot]: images/screenshot.png
[Next.js]: https://img.shields.io/badge/next.js-000000?style=for-the-badge&logo=nextdotjs&logoColor=white
[Next-url]: https://nextjs.org/
[Vue.js]: https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D
[Vue-url]: https://vuejs.org/
[Angular.io]: https://img.shields.io/badge/Angular-DD0031?style=for-the-badge&logo=angular&logoColor=white
[Angular-url]: https://angular.io/
[Svelte.dev]: https://img.shields.io/badge/Svelte-4A4A55?style=for-the-badge&logo=svelte&logoColor=FF3E00
[Svelte-url]: https://svelte.dev/
[Laravel.com]: https://img.shields.io/badge/Laravel-FF2D20?style=for-the-badge&logo=laravel&logoColor=white
[Laravel-url]: https://laravel.com
[Bootstrap.com]: https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white
[Bootstrap-url]: https://getbootstrap.com
[JQuery.com]: https://img.shields.io/badge/jQuery-0769AD?style=for-the-badge&logo=jquery&logoColor=white
[JQuery-url]: https://jquery.com 
