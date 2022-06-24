# uPersonalize

[![Release Badge](https://img.shields.io/badge/uPersonalize-v0.1.2-blueviolet)](https://github.com/rbaconsulting/uPersonalize/releases/latest)
[![Nuget Package](https://img.shields.io/badge/uPersonalize-0.1.2-blue)](https://www.nuget.org/packages/uPersonalize/)
[![License](https://img.shields.io/badge/License-MPL2.0-green)](https://github.com/rbaconsulting/uPersonalize/blob/main/LICENSE)

[![Package uPersonalize](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml)
[![Build Status](https://github.com/rbaconsulting/uPersonalize/actions/workflows/ci-build.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/pull-request.yml)

## Overview

uPersonalize is an Umbraco app plugin for personalizing your website in a content friendly way. With uPersonalize, backoffice users are able to configure specific components to render differently on a page depending on conditions sepcific to individual users on your website. For example, below is a demo of using the grid layout to hide text if a user has been to the home page 10 times. Installing uPersonalize includes a grid layout datatype, a property editor, and a settings dashboard to configure security settings. However, uPersonalize also provides C# and Javsacript service classes that can be utilized for custom implementations.

![grid-demo-backoffice](https://user-images.githubusercontent.com/104644210/175646664-f32f99f5-0274-4fc5-a6c0-3648e5741109.gif)
![grid-demo](https://user-images.githubusercontent.com/104644210/175617138-8baf37dc-3b2a-4502-8a0d-c09cfc99cf38.gif)

uPersonalize currently suppports the following personalization conditions:
- Whether a client's IP Address is similar
- Whether a client's device matches a specific type
- Whether a page has been visited
- Whether a page has been visited X number of time
- Whether an event has been triggered
- Whether an event has been triggered X number of time
- User's logged in status using OOTB member functionality

Once a condition has been met, the following actions are currently supported:
- Hide a specific element
- Show a specific element
- Apply additional CSS classes to an element


For further documentation on how to install and use uPersonalize, [checkout out our wiki pages](../../wiki).
If you have an issue or bug to report, you can raise the issue within the [issues area of this repository](../../issues).
Before doing so however, please check if your issue or bug has already been reported, and/or if is [currently being worked on within an active project.](../../projects)
