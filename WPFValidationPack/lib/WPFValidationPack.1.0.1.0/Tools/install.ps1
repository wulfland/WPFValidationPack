param($installPath, $toolsPath, $package, $project) 

function Set-FileBuildAction($project, $fileName, $buildAction, $customTool)
{
	$folder = $project.ProjectItems.Item("Resources")
	$file = $folder.ProjectItems.Item($fileName)
	$itemTypeProperty = $file.Properties.Item("ItemType")
	$itemTypeProperty.Value = $buildAction;
}

function Add-ResourceDictionary($project, $filename)
{
	$file = $project.ProjectItems.Item($fileName)
	$xml = [xml](Get-Content $file)

	$xml.Application.Application.Resources;

}

Set-FileBuildAction $project "CommonStyles.xaml" "Page" "MSBuild:Compile"
