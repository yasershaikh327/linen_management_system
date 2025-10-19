const apiUrl = "https://resturantapp-2z56.onrender.com/WeatherForecast";

function fetchWeather() {
    const xhr = new XMLHttpRequest();
    xhr.open("GET", apiUrl, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            const data = JSON.parse(xhr.responseText);
            const list = document.getElementById("forecastList");
            list.innerHTML = "";
            data.forEach(item => {
                const li = document.createElement("li");
                li.textContent = `${item.date} - ${item.temperatureC}°C`;
                list.appendChild(li);
            });
        }
    };
    xhr.send();
}

window.onload = fetchWeather;
