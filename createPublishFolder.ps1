param ($config)

$PUBLISH_DIR = "ShipExpander\bin\publish-$config"

Write-Output "1:    Creating initial folder structure"

Write-Output "Creating directory: $PUBLISH_DIR"
New-Item -Path "$PUBLISH_DIR" -ItemType "directory"


Write-Output "2:    Copying DLL"

Copy-Item -Path "ShipExpander\bin\$config\netstandard2.1\JBriggs.ShipExpander.dll" -Destination "$PUBLISH_DIR"

Write-Output "3:    Copying Assets"

Copy-Item -Path "ShipExpander\AssetBundles" -Destination "$PUBLISH_DIR\AssetBundles" -Recurse
