# uPersonalize Overview

[![Package uPersonalize](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml)
[![Build Status](https://github.com/rbaconsulting/uPersonalize/actions/workflows/ci-build.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/pull-request.yml)
[![Nuget Package](https://img.shields.io/badge/uPersonalize-nuget.org-blue)](https://www.nuget.org/packages/uPersonalize/)

uPersonalize brings personalization to Umbraco in a content friendly way! With uPersonalize, backoffice users are able to configure specific components to render differently on a page depending on conditions sepcific to individual users on your website. For example, below is a demo of using uPersonalize to hide text if a user has been to the home page 10 times.

_**Disclaimer: The recordings below contain different examples of content entry and personalization rules.**_
![grid-demo-backoffice](https://user-images.githubusercontent.com/104644210/175646664-f32f99f5-0274-4fc5-a6c0-3648e5741109.gif)
![grid-demo](https://user-images.githubusercontent.com/104644210/176764710-417dbeaa-65e7-4ca4-92e8-1d09df4de398.gif)

uPersonalize currently suppports the following personalization conditions:
- If a user's IP address matches an IP range (i.e. 31.10.xx.xxx would match UK IP addresses)
- If a user's device matches a specific type (i.e. Andriod vs iPhone)
- If a user has visited a specified page or visited the page X number of times
- If a user triggered an event or triggered it X number of times
- User's logged in status using OOTB member functionality
- If today's date and time is before or after a specified date and time.

Once a condition has been met, the following actions can be taken:
- Hide the personalized component
- Show the personalized component
- Add additional CSS classes the personalized component

For further documentation on how to install and use uPersonalize, [checkout out our wiki pages](../../wiki).
If you have an issue or bug to report, you can raise the issue within the [issues area of this repository](../../issues).
Before doing so however, please check if your issue or bug has already been reported, and/or if is [currently being worked on within an active project.](../../projects)
