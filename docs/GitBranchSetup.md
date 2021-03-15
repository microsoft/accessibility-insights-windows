<!--
Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the MIT License.
-->

## Git branch setup

This document describes how to set up your development environment and contribute changes to
[accessibility-insights-windows](https://github.com/Microsoft/accessibility-insights-windows). This document assumes basic working knowledge
of Git and related tools. The instructions are specific to this project.

## Creating your own fork

If you wish to contribute changes back to the [accessibility-insights-windows](https://github.com/Microsoft/accessibility-insights-windows)
repository, start by creating your own [Fork](https://help.github.com/en/articles/fork-a-repo) of the repository. This will keep down the number of branches on the main repository. In your own fork, you can create as many branches as you like.

-   Navigate to [GitHub](https://github.com/) with a browser and log in to your GitHub account. For the sake of this document, let's assume your username is **ada-cat**.
-   Navigate to the [accessibility-insights-windows](https://github.com/Microsoft/accessibility-insights-windows) repository in the same browser session.
-   Click on the **Fork** button at the top right corner of the page.
-   Create the fork under your account. Your GitHub profile should now show **accessibility-insights-windows** as one of your repositories.
-   Create a folder on your device and clone your fork of the **accessibility-insights-windows** repository. e.g. `https://github.com/ada-cat/accessibility-insights-windows`. Notice how your GitHub username is in the repository location.

```
git clone https://github.com/ada-cat/accessibility-insights-windows
```

## Setting up the upstream repository

Before starting to contribute changes, please setup your upstream repository to the
primary **accessibility-insights-windows** repository.

-   When you run git remote -v, you should see only your fork in the output list

```
git remote -v
origin  https://github.com/ada-cat/accessibility-insights-windows (fetch)
origin  https://github.com/ada-cat/accessibility-insights-windows (push)
```

-   Map the primary **accessibility-insights-windows** as the upstream remote

```
git remote add upstream https://github.com/Microsoft/accessibility-insights-windows
```

-   Now, running `git remote -v` should show the upstream repository also

```
git remote -v
origin  https://github.com/ada-cat/accessibility-insights-windows (fetch)
origin  https://github.com/ada-cat/accessibility-insights-windows (push)
upstream        https://github.com/Microsoft/accessibility-insights-windows (fetch)
upstream        https://github.com/Microsoft/accessibility-insights-windows (push)
```

-   At this point you are ready to start branching and contributing back changes.

## Making code changes and creating a pull request

Create a branch from your fork and start making the code changes. Once you are happy with the changes, and want to merge them to the main **accessibility-insights-windows** project, create a pull request from your branch directly to "Microsoft/accessibility-insights-windows main".

## Merging upstream main into your fork main

From time to time, your fork will get out of sync with the upstream remote. Use the following commands to get the main branch of your fork up to date.

```
git fetch upstream
git checkout main
git pull upstream main
git push
```

## Merging upstream main into your current branch

Use these commands instead if you would like to update your current branch in your fork from the upstream remote.

```
git fetch upstream
git pull upstream main
git push
```
