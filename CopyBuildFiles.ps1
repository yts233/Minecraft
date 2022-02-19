foreach($file in (Get-ChildItem -Recurse -Filter "bin")){
    Copy-Item $file.FullName ("bin/"+ $file.Parent.Name)
}