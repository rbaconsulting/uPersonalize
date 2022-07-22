# uPersonalize Overview

[![Package uPersonalize](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml)
[![Build Status](https://github.com/rbaconsulting/uPersonalize/actions/workflows/ci-build.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/pull-request.yml)
[![Nuget Package](https://img.shields.io/badge/uPersonalize-nuget.org-blue)](https://www.nuget.org/packages/uPersonalize/)

uPersonalize is an Umbraco app plugin for personalizing your website in a content friendly way. With uPersonalize, backoffice users are able to configure specific components to render differently on a page depending on conditions sepcific to individual users on your website. For example, below is a demo of using the grid layout to hide text if a user has been to the home page 10 times. Installing uPersonalize includes a grid layout datatype, a property editor, and a settings dashboard to configure security settings. However, uPersonalize also provides C# and Javsacript service classes that can be utilized for custom implementations.

_**Disclaimer: The recordings below contain different examples of content entry and personalization rules.**_
![grid-demo-backoffice](https://user-images.githubusercontent.com/104644210/175646664-f32f99f5-0274-4fc5-a6c0-3648e5741109.gif)
![grid-demo](https://user-images.githubusercontent.com/104644210/176764710-417dbeaa-65e7-4ca4-92e8-1d09df4de398.gif)

uPersonalize currently suppports the following personalization conditions:
- User's IP address matching an IP range (i.e. 31.10.xx.xxx would match UK IP addresses)
- User's device matching a specific type (i.e. Andriod vs iOS)
- User visiting a certain page or not
- User visiting a certain page X number of times
- User triggering an event
- User triggering an event X number of times
- User's logged in status using OOTB member functionality

Once a condition has been met, the following actions can be taken:
- Hide the personalized component
- Show the personalized component
- App additional CSS classes the personalized component


For further documentation on how to install and use uPersonalize, [checkout out our wiki pages](../../wiki).
If you have an issue or bug to report, you can raise the issue within the [issues area of this repository](../../issues).
Before doing so however, please check if your issue or bug has already been reported, and/or if is [currently being worked on within an active project.](../../projects)
