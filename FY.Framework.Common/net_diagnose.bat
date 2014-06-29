@echo off
echo 请您稍候，正在执行网络探测

ipconfig /all >> result.log
echo 已完成 30％

ping www.baidu.com >> result.log
echo 已完成 60％

tracert www.baidu.com >> result.log
echo 已完成 100％



echo 探测完毕，请您将同文件夹下生成的文件result.log发给我们，谢谢！
pause