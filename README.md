# Mars
Mars Rover Image Viewer Solution

## Details
Solution uses Mars Rover Photos API described [here](https://api.nasa.gov) with a given day as input, returning corresponding photo images as output. The application downloads and store each image locally in your C:/Mars (configurable) drive path.

- You may use the list of date formats below to pull the images that were captured on that date:
  - 02/27/17
  - June 2, 2018
  - Jul-13-2016
  - April 31, 2018
- Application is ASP.net MVC and language is C#/.NET Core on the backend
- You may only use API called MarsRoverPhotos by calling https://localhost:44348/MarsRoverPhotos/[date] in case if you want to use it in your existing solution.
- Application displays all images for given earth day in a web browser along with data about each image. 

