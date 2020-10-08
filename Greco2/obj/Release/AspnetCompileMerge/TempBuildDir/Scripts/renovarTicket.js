
function verificarTiempoRestante() {
    var diferencia = getDuracionMaxima();
    //if (getTiempoRestante() > 600000 && elModalEstaCerrado) {
    //alert(elModalEstaCerrado);
    if (diferencia > 1200000 /*&& elModalEstaCerrado*/) {
        openModalRenew();
    } else {
        initIntervalo();
    }
}


function Volver() {
    history.back();
    verificarTiempoRestante();
}

var diferencia;



//$(document).ajaxComplete(function () {
//    horario = Date.now();
//    localStorage.setItem("horarioUltimaPeticion", horario);
//    var z = localStorage.getItem("horarioUltimaPeticion");
//    toastr.success("horarioUltimaPeticion : " + new Date(parseInt(z)));
    
//});


function guardarHorario() {
    horario = Date.now();
    localStorage.setItem("horario", horario);
    var x = localStorage.getItem("horario");
    var y = localStorage.getItem("horarioInicial");
    var diferencia = x - y;
   
}

function getDuracionMaxima() {
    horarioActual = Date.now();
    var horarioInicial = localStorage.getItem("horarioInicial");
    var diferencia = horarioActual - horarioInicial;
    return diferencia;
}

function setHorarioInicial(unHorario) {
    localStorage.setItem("horarioInicial", unHorario);
    var nuevoHorarioInicial = localStorage.getItem("horarioInicial");
    //toastr.info("nuevoHorarioInicial :</br>" + nuevoHorarioInicial);
    //toastr.info(Date.now);
}


var intervalo;
var elModalEstaCerrado;
var laCuentaRegresivaContinua;

function initIntervalo() {
    //toastr.warning("Se ha iniciado el Intervalo");
    elModalEstaCerrado = true;
   
    valorInicial = 300;
    intervalo = setInterval(function () {
        //var antes = localStorage.getItem("horarioUltimaPeticion");
        var antes = localStorage.getItem("horarioInicial");
        var ahora = Date.now();
        diferencia = ahora - antes;
        //toastr.warning(diferencia);
        
        if ((getDuracionMaxima() >= 1200000) && elModalEstaCerrado) {
            getToastrRenewModal();
            laCuentaRegresivaContinua = true;
            elModalEstaCerrado = false;
            valorInicial = 300;
            tiempo = 0;
            ajustarConteo();
        } 
    },60000);

}

function openModalRenew() {
    getToastrRenewModal();
    laCuentaRegresivaContinua = true;
    elModalEstaCerrado = false;
    valorInicial = 300;
    tiempo = 0;
    ajustarConteo();
}

var valorInicial;
var tiempo;
var cuentaRegresiva;
function ajustarConteo() {
    valorInicial = valorInicial - 1;
    cuentaRegresiva = setTimeout(function () {
        ajustarConteo();
    }, tiempo)

    tiempo = 1000;
    //if (valorInicial <= 0 /*&& (getDuracionmaxima() > 600000*/ /*|| diferencia >= 600000*/) {
    if (valorInicial <= 0 && laCuentaRegresivaContinua) {

        clearTimeout(cuentaRegresiva);
        clearInterval(intervalo);
        valorInicial = 300;
        elModalEstaCerrado = true;
        laCuentaRegresivaContinua = false;
        BackToLogin();
    }

    else {
        if (!elModalEstaCerrado && (valorInicial >= 1) && laCuentaRegresivaContinua) {
            decrementar(valorInicial);
        }

    }

}
function getTiempoRestante() {
    var antes = localStorage.getItem("horarioInicial");
    var ahora = Date.now();
    var tiempoRestante = ahora - antes;
    return tiempoRestante;
}

function stopTimer() {
    clearInterval(intervalo);
}


function clearTimer() {
    clearInterval(intervalo);
    horario = Date.now();
    localStorage.setItem("horarioUltimaPeticion", horario);
    
}

function getToastrRenewModal() {
    elModalEstaCerrado = false;
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": 0,
        "extendedTimeOut": 0,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut",
        "tapToDismiss": false
    }
    Command: toastr["success"]("Si desea extender la Sesión<br />Presione Continuar<br /><div id='conteo'></div><br /><br /><button type='button' class='btn clear' onclick='renovar()'>Continuar</button>");
    
}



function decrementar(valor) {
   
    $('#conteo').html("");
    //console.log(valor);
    $('#conteo').html(valor);
    
}


 