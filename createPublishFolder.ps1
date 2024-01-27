param ($config)

$PUBLISH_DIR = "bin\publish-$config"

Write-Output "1:    Creating initial folder structure"

New-Item -Path "$PUBLISH_DIR" -ItemType "directory"


Write-Output "2:    Copying DLL"

Copy-Item -Path "ShipExpander\bin\$config\netstandard2.1\JBriggs.ShipExpander.dll" -Destination "$PUBLISH_DIR"

Write-Output "3:    Copying Assets"

Copy-Item -Path "ShipExpander\AssetBundles" -Destination "$PUBLISH_DIR\AssetBundles" -Recurse
