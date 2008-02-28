@echo off

echo Merging dll's

mkdir Temp

"..\..\lib\ILMerge.exe" /ndebug /out:"Temp\N2.Templates.UI.dll" "..\Output\Templates\Bin\N2.Templates.UI.dll" /log "..\Output\Templates\Bin\N2.Templates.Calendar.dll" "..\Output\Templates\Bin\N2.Templates.Faq.dll" "..\Output\Templates\Bin\N2.Templates.Form.dll" "..\Output\Templates\Bin\N2.Templates.ImageGallery.dll" "..\Output\Templates\Bin\N2.Templates.News.dll" "..\Output\Templates\Bin\N2.Templates.Poll.dll" "..\Output\Templates\Bin\N2.Templates.RSS.dll" "..\Output\Templates\Bin\N2.Templates.Search.dll" "..\Output\Templates\Bin\N2.Templates.Survey.dll" 
del "..\Output\Templates\Bin\N2.Templates.UI.dll" 
del "..\Output\Templates\Bin\N2.Templates.Calendar.dll" 
del "..\Output\Templates\Bin\N2.Templates.Faq.dll" 
del "..\Output\Templates\Bin\N2.Templates.Form.dll" 
del "..\Output\Templates\Bin\N2.Templates.ImageGallery.dll" 
del "..\Output\Templates\Bin\N2.Templates.News.dll" 
del "..\Output\Templates\Bin\N2.Templates.Poll.dll" 
del "..\Output\Templates\Bin\N2.Templates.RSS.dll" 
del "..\Output\Templates\Bin\N2.Templates.Search.dll" 
del "..\Output\Templates\Bin\N2.Templates.Survey.dll" 
move "Temp\N2.Templates.UI.dll" "..\Output\Templates\Bin\" 

move Temp\N2.Edit.dll ..\Output\Core\Bin\

echo Done

pause