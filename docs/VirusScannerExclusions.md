<!--
Copyright (c) Microsoft Corporation. All rights reserved.
Licensed under the MIT License.
-->

## Virus scanner exclusions

When running locally, virus scanners have been known to have a significant impact on execution time. To prevent this, you may wish to exclude the entire repo from virus scanning. The specifics of how to do this will vary between virus scanners, between operating systems, and possibly between different versions of the same virus scanner.

### Microsoft Defender (Windows only)
The following steps allow you to exclude a folder and all its descendants from Microsoft Defender. These instructions have been validated on both Windows 10 and Windows 11:
- From the start menu, run **Windows Security**
- Enter the **Virus & threat protection** tab
- Under **Virus & threat protection settings**, click **Manage settings**
- Under **Exclusions**, click **Add or remove exclusions**
- If prompted for permissions, grant permissions
- Click **Add an exclusion**
- Select **Folder** as the exclusion type
- Specify the folder to ignore--all descendants of this folder will be ignored
- Click **Select folder**

### Other antivirus products
Please consult your antivirus product's documentation.