@echo off

echo Merging dll's

"..\..\lib\ILMerge.exe" /ndebug /out:"N2.Templates.UI.dll" "Deploy\Bin\N2.Templates.UI.dll" /log "Deploy\Bin\N2.Templates.Advertisement.dll" "Deploy\Bin\N2.Templates.Calendar.dll" "Deploy\Bin\N2.Templates.Faq.dll" "Deploy\Bin\N2.Templates.Form.dll" "Deploy\Bin\N2.Templates.ImageGallery.dll" "Deploy\Bin\N2.Templates.News.dll" "Deploy\Bin\N2.Templates.Poll.dll" "Deploy\Bin\N2.Templates.RSS.dll" "Deploy\Bin\N2.Templates.Scrum.dll" "Deploy\Bin\N2.Templates.Search.dll" 
move "Deploy\Bin\N2.Templates.Security.dll" .
move "Deploy\Bin\N2.Templates.Survey.dll" .
move "Deploy\Bin\N2.Templates.Syndication.dll" .
move "Deploy\Bin\N2.Templates.dll" .
move "Deploy\Bin\N2.Templates.SEO.dll" .

del "Deploy\Bin\N2.Templates*.dll" 

move N2.Templates*.dll Deploy\Bin\

echo 
echo Done!

pause