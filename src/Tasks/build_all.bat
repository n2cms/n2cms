@ECHO OFF
@ECHO BUILDING TO ..\OUTPUT

@ECHO CLEANING...
:clean_output.bat

@ECHO BUILDING CORE
build_core.bat

@ECHO COPYING CORE TO ..\OUTPUT
output_core.bat

@ECHO MERGING EDIT ASSEMBLIES
ilmerge_core_output.bat

@ECHO BUILDING TEMPLATES
build_templates.bat

@ECHO COPYING CORE TO ..\OUTPUT
output_templates.bat

PAUSE