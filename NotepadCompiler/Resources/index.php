<html>
<style>
body{
    overflow-x:hidden;
    overflow-y:hidden;
	font-family:'-apple-system','HelveticaNeue'; font-size:19;
}
</style>
<script type="text/javascript">
window.onload = function() {
    var frameRefreshInterval = setInterval(function() {
        document.getElementById("myframe").src = document.getElementById("myframe").src
    }, 500);
}
</script>
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<body style="margin:0px; background-color:rgb(80,80,80)">
<center>
<div style="width:100%; background-color: rgb(40,40,40); color:white; padding:10px">
<h1 style="height:25px">NotepadCompiler</h1>


</div>



<iFrame style="border:none;width:80vw;height:calc(100vh - 40px);" id="myframe" src="NPC_content.html"/>



</center>
</body>
</html>