CONFIG="$1"
PUBLISH_DIR="bin\publish-$CONFIG"

echo "$PUBLISH_DIR"

echo '1:    Creating initial folder structure'
mkdir "$PUBLISH_DIR"

echo '2:    Copying DLL'
cp "bin\$CONFIG\netstandard2.1\JBriggs.ShipExpander.dll" "$PUBLISH_DIR"

echo '3:    Copying Assets'
cp -r ./AssetBundles "$PUBLISH_DIR"