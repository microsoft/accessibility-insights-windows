## Test Scenarios
To make sure that a PR doesn't introduce a major regression, please run through the following scenarios locally before a PR is created.

- Inspect in Live
  - Select an element via the mouse, keyboard, and tree navigation in AI for Windows (Hover over the element to be selected, use arrow keys to select the target element in AI for Windows).
    - Verify that the selected element shows up as highlighted in the target app.
- Test by using the shortcut for "Run tests on the selected element"
  - By default this is `Shift + F8`. The current list of shortcuts can be found under `Settings > Application > Shortcuts`.
  - Hover over an element that needs to be tested and use the shortkey to invoke the accessibility tests.
  - Walk the hierarchy tree using the keyboard as well as the mouse.
- Test mode in the Application mode.
  - In the "What to select" drop down, select "Entire app".
  - Hover over and select a target app. Click on the beaker icon to kick off an accessibility pass.
  - Save test results.
  - File a bug from test mode as well as the hierarchy tree. (Optional, requires AzureDevOps team + account).
  - Go to the "Tab Stops" tab and run the tab stop test. Ensure that the tab stops list is populated when you tab around the application.
- Load test results and inspect further.
  - From the "Live Inspect" view, load a saved file using the "Load file" button -- it should match what you just saved.
  - Select an instance of a failure and verify that the "Inspect" view is populated with details for that failure.
- Snippet
  - After running a test you should be in the test mode. 
  - In the test mode, click on an error result list item (actual error). This should take you to the "Tree" view.
  - Verify you see corresponding snippets and that links in the "How to fix" subtab work.
- Event
  - Select a target element for testing. This will take the Accessibility Insights for Windows application to the "Live Inspect" view. 
  - In the hierarchy tree, click on the ellipsis (...) to open more options. 
  - Click on the "Listen to events" option.
  - Select the events that need to be recorded and start recording.
  - Play with the target element.
  - Stop recording in the Accessibility Insights for Windows app.
  - Save events to disk using the "Save" button.
  - Go to live inspect mode.
  - Load events file using the "File Open" button -- it should match what you just saved.

### Additional checks for UI related changes
- Run the production version of Accessibility Insights for Windows against an instance of Accessibility Insights for Windows with the new changes.
  - Look for any new accessibility issues that might have been introduced by local changes.
- Verify that guidelines for [High Contrast](HighContrastSupport.md) have been adhered to.
