# Ofqual Register of Regulated Qualifications Frontend 

The Ofqual Register of Regulated Qualifications frontend allows users to:

- Find a regulated qualification
- Find a regulated awarding organisation

## Provider 
[The Office of Qualifications and Examinations Regulation](https://www.gov.uk/government/organisations/ofqual)

## About this project
This project is a ASP.NET Core 8 web app with the MVC architecture utilizing Docker for deployment. The web app runs on an App service for Container apps on Azure

#### Libraries
- Refit v7.0
- CSVHelper v32.0.1
- LigerShark.WebOptimizer v3.0.405 (for auto minification of assets)


## Architecture
![frontend](https://github.com/OfqualGovUK/ofqual-register-frontend/blob/main/Frontend_Arch.jpg?raw=true)

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

A pipeline (`azure-pipelines.yml`) is set on DevOps to automatically deploy the Web App to an Azure container registry . A webhook is setup to pull the container image into an App Service for continuous deployment.

## Full Data Download

A DownloadsController has been created to download the full Qualifications and Organisations data in CSV format. The CSVs are stored in a storage container on Azure as blobs. The code checks the last time CSVs were modified and fetches new data from the Database if the blobs are older than a day. 

## Qualificaions Sitemap

As the list of qualifications can be huge (48k at the time of writing), the same methodology as Full Data Download is used to store the qualification titles and names, into a JSON file. This file is updated if the last modified date on the file was older than a week. 
