# WeatherApp

Created by Joseph Hargus
For Mizzou's IT4400
04/29/2024

## Project Description

This application is a weather app that displays various information for a location of the users choice. The application will show current conditions, hourly and daily forecast, and any weather alerts issued for that area.

The user can change their location by clicking “settings”->”location” in the menu bar. A new window will appear, allowing the user to add and delete locations, and change locations. The user can add a new location by name, zip code, coordinates, etc.

The project is powered by weatherapi.com, a free weather API. The API is used to search locations and gather weather data. The API includes a folder filled with icons for weather conditions.

The project also uses Newtonsoft.Json to serialize and deserialize class objects. To save/load the locations that the user has added.
