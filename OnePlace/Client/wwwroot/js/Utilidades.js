/*aqui creamos el metodo customconfirm para editar a nuestro gusto el sweetalert y poder pasar los parametros necesarios*/
function CustomConfirm(titulo, mensaje, tipo) {
    //retornamos una promesa
    return new Promise((resolve) => {
        Swal.fire({
            title: titulo,//le pasamos el titulo
            text: mensaje,//le pasamos el mensaje
            icon: tipo,//le pasamos el tipo questions,info,warning
            showCancelButton: true,
            confirmButtonColor: '#02889B',
            cancelButtonColor: '#dc3545',
            confirmButtonText: 'Confirmar'
        }).then((result) => {
            //resolve true es que la promesa retorno exitosamente el valor en este caso true
            if (result.value) {
                resolve(true);
            }
            else {
                resolve(false);
            }
        });
    });
}

/*funcion para mostrar y ocultar contraseña en login y registro*/
function MostrarPassword()
{
    var cambio = document.getElementById("txtPassword");

    if (cambio.type == "password")
    {
        cambio.type = "text";
        $('.iconover').removeClass('oi oi-lock-locked').addClass('oi oi-lock-unlocked');
    }
    else
    {
        cambio.type = "password";
        $('.iconover').removeClass('oi oi-lock-unlocked').addClass('oi oi-lock-locked');
    }
};

/*funcion para mostrar animacion de numeros en acendente*/
function PureCounter() {

    const scr1 = document.createElement("script")
    scr1.src = "lib/purecounter_vanilla.js"

    setTimeout(function () {
        document.getElementById("counter2").classList.add("purecounter")
        let head = document.querySelector("head")
        head.appendChild(scr1)
    }, 2000);
    
};

/*ocultar boton derecho para evitar descargar archivos*/
function BloquearBotonDerecho() {

    $(document).bind("contextmenu", function (e) {
        return false;
    });   
};

/*hacer funcionar el boton up*/
function BotonUP() {

    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 1500, 'easeInOutExpo');
        return false;
    });
};

/*hacer funcionar el carusel*/
function StarCarousel() {
    // Testimonials carousel
    $(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        margin: 10,
        dots: false,
        loop: true,
        center: true,
        responsive: {
            0: {
                items: 1
            },
            576: {
                items: 1
            },
            768: {
                items: 2
            },
            992: {
                items: 5
            }
        }
    });
};

function StarCarousel2() {
    // curso carousel
    $('.curso-carousel').owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        margin: 15,
        dots: false,
        loop: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 2
            },
            1000: {
                items: 3
            }
        }
    });
};

function StarCarousel3() {
    // cumple carousel
    $('.cumple-carousel').owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        dots: false,
        loop: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 2
            },
            1000: {
                items: 3
            }
        }
    });
};

function StarCarousel4() {
    // promociones carousel
    const swiper = new Swiper('.swiper', {
        // Optional parameters
        direction: 'horizontal',
        loop: true,
        autoplay: {
            delay: 2500,
            disableOnInteraction: false,
        },
        /*  // If we need pagination
         pagination: {
           el: '.swiper-pagination',
         }, */
        /*  breakpoints: {
             // when window width is >= 640px
             600: {
                 width: 640,
                 slidesPerView: 1,
             },
             // when window width is >= 768px
             1000: {
                 width: 1349,
                 slidesPerView: 1,
             }
         },  */
    });
};


function Sticky() {

    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 45) {
            $('.navbar').addClass('sticky-top shadow-sm');
        } else {
            $('.navbar').removeClass('sticky-top shadow-sm');
        }
    });
};

function Animacion() {

    new WOW().init();
};

function loadCss(cssPath) {

    var link = document.createElement('link');
    link.rel = "stylesheet";
    link.type = "text/css";
    link.href = cssPath

    document.head.appendChild(link);
};






