//document.body.appendChild(document.createElement('script')).src = 'http://localhost:3622/trans.js/'

function addjq()
{
    document.body.appendChild(document.createElement('script')).src = 'http://code.jquery.com/jquery-latest.js';
}

function test()
{
jQuery.ajax({
type: 'POST', 
contentType: 'application/json',
url: 'http://localhost:3622/Trans.asmx/HelloWorld', 
data: '{}',
dataType: 'json',
success: function(str){
alert(str.toLocaleString());
jQuery('#source').val(jQuery(str).text());
}, 
error: function(e){alert(e)}
});
}
function reVal(d) {
return jQuery(d).attr('d');
}
function getText()
{
jQuery.ajax({
type: 'POST', 
contentType: 'application/json',
url: 'http://localhost:3622/Trans.asmx/GetText', 
data: '{index: 1}',
dataType: 'json',
success: function(str){
jQuery('#source').val(reVal(str));
sendText();
}, 
error: function(e){
alert(e)
}
});
}
function sendText()
{
jQuery.ajax({
type: 'POST', 
contentType: 'application/json',
url: 'http://localhost:3622/Trans.asmx/RecText', 
data: '{tran: "text"}', 
dataType: 'json', 
success: function(str){
jQuery('#result_box').text(reVal(str));
}
});
}