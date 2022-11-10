Param(
	[Parameter(Mandatory=$false)][string] $Directory
)

$sl = Get-Location
Set-Location (Split-Path $MyInvocation.MyCommand.Path)

if ($Directory -ieq $null -or $Directory.Length -ieq 0)
{
	$Directory = "E:\Downloads\NG\_Decoded"
}

Write-Host "Running in $Directory"
Write-Output "Running in $Directory"

## 32 to 126
function RemoveForeignChars ([string]$s) 
{
	$r = ""
	for ($i = 0; $i -lt $s.Length; $i++)
	{
		if ([int]($s[$i]) -ge 32 -and [int]($s[$i]) -le 126 -and $s[$i] -ne ' ' -and $s[$i] -ne '%' -and $s[$i] -ne ']' -and $s[$i] -ne '[')
		{
			$r += $s[$i]
		}
		else
		{
			$r += '.'
		}
	}
	
	if ($r.Length -eq 0)
	{
		$r = [guid]::NewGuid().ToString()
	}
	
	if ($s -ne $r)
	{
		Write-Host $s changed to $r
	}
	
	return $r
}

function RemoveForeignCharsFromDir () 
{
	foreach ($d in Get-ChildItem -Directory -Recurse)
	{
		$nn = RemoveForeignChars $d.BaseName
		
		if ($nn -ne $d.BaseName)
		{
			$ofn = $d.FullName
			
			if (Test-Path -LiteralPath "$ofn")
			{
				Rename-Item -LiteralPath $d.FullName -NewName "$nn"
			}
		}
	}
	
	foreach ($f in Get-ChildItem -File -Recurse)
	{
		$nn = RemoveForeignChars $f.BaseName
		
		if ($nn -ne $f.BaseName)
		{
			$ofn = $f.FullName
			$nfn = $nn + $f.Extension
			
			if (Test-Path -LiteralPath "$ofn")
			{
				Rename-Item -LiteralPath "$ofn" -NewName "$nfn"
			}
		}
	}
}

Set-Location $Directory 
RemoveForeignCharsFromDir
Set-Location $sl

Write-Host "Done"

