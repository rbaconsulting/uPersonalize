# uPersonalize

[![Release Badge](https://img.shields.io/badge/uPersonalize-v0.1.2-blueviolet)](https://github.com/rbaconsulting/uPersonalize/releases/latest)
[![Nuget Package](https://img.shields.io/badge/uPersonalize-0.1.2-blue)](https://www.nuget.org/packages/uPersonalize/)
[![License](https://img.shields.io/badge/License-MPL2.0-green)](https://github.com/rbaconsulting/uPersonalize/blob/main/LICENSE)

[![Package uPersonalize](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/package-uPersonalize.yml)
[![Build Status](https://github.com/rbaconsulting/uPersonalize/actions/workflows/ci-build.yml/badge.svg)](https://github.com/rbaconsulting/uPersonalize/actions/workflows/pull-request.yml)

## Overview

uPersonalize is an app plugin adding personalization options to Umbraco. The goal is to allow content authors to easily personalize content, so that variations of
content can be shown to different website users. The app plugin was intended to be used by the grid layout editor, however, the javascript and C# API can
be extended to be used outside of the grid layout editor.

![ezgif com-gif-maker (1)](https://user-images.githubusercontent.com/104644210/175616952-333ddd5f-1691-4e9a-a541-763b3532336c.gif)

![ezgif com-gif-maker](https://user-images.githubusercontent.com/104644210/175616961-af252557-3eb2-4844-b9d1-4fb9758a5bf3.gif)

uPersonalize currently suppports the following personalization conditions:
- Whether a client's IP Address is similar
- Whether a client's device matches a specific type
- Whether a page has been visited
- Whether a page has been visited X number of time
- Whether an event has been triggered
- Whether an event has been triggered X number of time


Once a condition is met, the following actions can be taken within the grid layout editor:
- Hide a specific element
- Show a specific element
- Apply additional html classes to an element


For further documentation on how to install and use uPersonalize, [checkout out our wiki pages](../../wiki).
If you have an issue or bug to report, you can raise the issue within the [issues area of this repository](../../issues).
Before doing so however, please check if your issue or bug has already been reported, and/or if is [currently being worked on within an active project.](../../projects)
