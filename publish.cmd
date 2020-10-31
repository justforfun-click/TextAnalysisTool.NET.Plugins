@echo off
md bin\publish
copy bin\Release\*.dll bin\publish
copy TextAnalysisTool.NET\* bin\publish