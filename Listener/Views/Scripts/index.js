function showLogin(){
    $( "#dialog" ).dialog({
          resizable: false,
          height: 220,
          width: 400,
          modal: true,
          buttons: {
            "  Login  ": function () {
              $.cookie('cloud_user_cookie', $('#username').val());
              $(this).dialog("close");
              location.reload();
            },
            "  Cancel  ": function() {
              $( this ).dialog( "close" );
            }
          }});
}

function init() {
  console.log("222");
  $('#signin').click(function (e) {
    event.stopPropagation();
    showLogin();
    return false;
  });

  $('#signout').click(function (e) {
    event.stopPropagation();
    $.cookie('cloud_user_cookie', "");
    location.reload();
    return false;
  });
}


jQuery(document).ready(init)