# Ofqual Register of Regulated Qualifications Frontend 

The Ofqual Register of Regulated Qualifications frontend allows users to:

-Find a regulated qualification
-Find a regulated awarding organisation

## Provider 
[The Office of Qualifications and Examinations Regulation](https://www.gov.uk/government/organisations/ofqual)

## About this project
This project is a ASP.NET Core 8 web app with the MVC architecture utilizing Docker for deployment. The web app runs on an App service for Container apps on Azure

#### Libraries
- Refit v7.0
- CSVHelper v32.0.1
- LigerShark.WebOptimizer v3.0.405 (for auto minification of assets)


## Architecture
![frontend](https://github.com/OfqualGovUK/ofqual-register-frontend/blob/main/API_Arch.jpg?raw=true)

## Environment Variables / App settings
Variables set in the Function Apps config on Azure

- `RegisterAPIUrl`: URL for the Register API 
- `RefdataAPIUrl`: URL for the Ref Data API to fetch filter values for qualifications (qualificationtypes, levels, SSAs and assessment methods)
- `OrganisationsPagingLimit`: Number of items on the Organisations search results page
- `QualificationsPagingLimit`: Number of items on the Qualifications search results page

## Assets

- Analytics JS: `wwwroot/js/analytics.js`
- Qualifications Search page JS: `wwwroot/js/qualSearchResults.js`
- Site CSS: `wwwroot/css/application.css`, `wwwroot/css/site.css`

## APIs

- RefData API: used to get the filter values for filters on the qualifications search results page for Qualificaion Types, Levels, SSA and Assessment Methods
    
- Register API: Qualifications and Organisations data

## Deployment

A pipeline is set on DevOps to automatically deploy the Web App to an Azure container registry (`azure-pipelines`). A webhook is setup to pull the container image into an App Service for continuous deployment.