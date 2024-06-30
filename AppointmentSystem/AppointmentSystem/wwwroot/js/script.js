// JavaScript Document
$(function() { //JS開頭

    var WINDOW = $(window).width(); //視窗寬度
    var WINDOWH = $(window).height(); //視窗高度

    //----------------gotop功能-----------------
    $("#gotop").click(function() {
        $("html,body").animate({ scrollTop: 0 }, 300);
        return false;
    })

    $("#mobile-gotop").click(function() {
        $("html,body").animate({ scrollTop: 0 }, 300);
        return false;
    })

    $(window).scroll(function() {
        if ($(this).scrollTop() > 100) { //若目前的位置距離網頁頂端>100px
            $("#gotop").fadeIn("fast");
        } else {
            $("#gotop").stop().fadeOut("fast");
        }
    });

    //--------------------- menu 伸縮 ----------------------

    $(".menu-control").click(function() {
        if ($(window).width() > 576) {
            if ($(".main-left").hasClass('off')) {
                $(".main-left").animate({ left: 0 }, 200);
                $(".main-left").removeClass('off');
                $(this).find('i').removeClass('fa-chevron-right');
                $(this).find('i').addClass('fa-chevron-left');
                $(".main-right").animate({ "marginLeft": 230 }, 500);
            } else {
                $(".main-left").animate({ left: -204 }, 100);
                $(".main-left").addClass('off');
                $(this).find('i').removeClass('fa-chevron-left');
                $(this).find('i').addClass('fa-chevron-right');
                $(".main-right").animate({ "marginLeft": 30 }, 500);
            }
        } else {
            if ($(".main-left").hasClass('off')) {
                $(".main-left").animate({ left: 0 }, 200);
                $(".main-left").removeClass('off');
                $(this).find('i').removeClass('fa-chevron-right');
                $(this).find('i').addClass('fa-chevron-left');

            } else {
                $(".main-left").animate({ left: -204 }, 100);
                $(".main-left").addClass('off');
                $(this).find('i').removeClass('fa-chevron-left');
                $(this).find('i').addClass('fa-chevron-right');
                $(".main-right").animate({ "marginLeft": 30 }, 500);
            }
        }

    });

    //---------------------callapse展開----------------------

    $("div[id^=heading] > h4 > a").click(function() {
        if ($(this).hasClass('collapsed')) {
            $("div[id^=heading] > h4 > a").removeClass('chosen');
            $(this).addClass('chosen');
        } else {
            $(this).removeClass('chosen');
        }
    });

    // --- S進度曲線 ---

    Highcharts.chart('containerS', {
        chart: {
            type: 'spline'
        },
        title: {
            text: 'S進度曲線標題'
        },
        subtitle: {
            text: '子標題'
        },
        xAxis: {
            type: 'datetime',
            dateTimeLabelFormats: { // don't display the dummy year
                month: '%Y / %m / %d',
                year: '%b'
            },
            title: {
                text: 'Date'
            }
        },
        yAxis: {
            title: {
                text: '累計完成 (%)'
            },
            min: 0
        },
        tooltip: {
            headerFormat: '<b>{series.name}</b><br>',
            pointFormat: '{point.x:%Y年%m月%d日}: {point.y:.2f} %'
        },

        plotOptions: {
            series: {
                marker: {
                    enabled: true
                }
            }
        },

        colors: ['#6CF', '#06C', '#036', '#39F', '#000'],

        // Define the data points. All series have a dummy year
        // of 1970/71 in order to be compared on the same x axis. Note
        // that in JavaScript, months start at 0 for January, 1 for February etc.
        series: [{
            name: "預定累計完成",
            data: [
                [Date.UTC(2018, 7, 12), 0.13],
                [Date.UTC(2018, 7, 19), 0.29],
                [Date.UTC(2018, 7, 26), 0.47],
                [Date.UTC(2018, 8, 2), 0.86],
                [Date.UTC(2018, 8, 9), 1.25],
                [Date.UTC(2018, 8, 16), 1.89],
                [Date.UTC(2018, 8, 23), 3.15],
                [Date.UTC(2018, 8, 30), 5.26],
                [Date.UTC(2018, 9, 7), 7.98],
                [Date.UTC(2018, 9, 14), 10.3],
                [Date.UTC(2018, 9, 21), 13.48],
                [Date.UTC(2018, 9, 28), 17.16],
                [Date.UTC(2018, 10, 4), 21.62],
                [Date.UTC(2018, 10, 11), 26.07],
                [Date.UTC(2018, 10, 18), 30.53],
                [Date.UTC(2018, 10, 25), 35.15],
                [Date.UTC(2018, 11, 2), 42.56],
                [Date.UTC(2018, 11, 9), 50.63],
                [Date.UTC(2018, 11, 16), 59.22],
                [Date.UTC(2018, 11, 23), 67.58],
                [Date.UTC(2018, 11, 30), 75.37],
                [Date.UTC(2019, 0, 6), 81.93],
                [Date.UTC(2019, 0, 13), 88.18],
                [Date.UTC(2019, 0, 20), 93.44],
                [Date.UTC(2019, 0, 27), 97.65],
                [Date.UTC(2019, 1, 3), 100]
            ]
        }, {
            name: "累計完成",
            data: [
                [Date.UTC(2018, 7, 12), 0],
                [Date.UTC(2018, 7, 19), 0],
                [Date.UTC(2018, 7, 26), 0],
                [Date.UTC(2018, 8, 2), 0],
                [Date.UTC(2018, 8, 9), 0],
                [Date.UTC(2018, 8, 16), 0],
                [Date.UTC(2018, 8, 23), 0.2],
                [Date.UTC(2018, 8, 30), 0.2],
                [Date.UTC(2018, 9, 7), 2.42],
                [Date.UTC(2018, 9, 14), 2.29],
                [Date.UTC(2018, 9, 21), 8.45],
                [Date.UTC(2018, 9, 28), 14.65],
                [Date.UTC(2018, 10, 4), 17.55],
                [Date.UTC(2018, 10, 11), 25.63],
                [Date.UTC(2018, 10, 18), 30.41],
                [Date.UTC(2018, 10, 25), 30.97],
                [Date.UTC(2018, 11, 2), 30.97],
                [Date.UTC(2018, 11, 9), 37.07],
                [Date.UTC(2018, 11, 16), 51.3],
                [Date.UTC(2018, 11, 23), 72.1],
                [Date.UTC(2018, 11, 30), 77.64],
                [Date.UTC(2019, 0, 6), 78.37],
                [Date.UTC(2019, 0, 13), 79.65],
                [Date.UTC(2019, 0, 20), 99.5],
                [Date.UTC(2019, 0, 27), 100],
                [Date.UTC(2019, 1, 3), 100]
            ]
        }],

        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    plotOptions: {
                        series: {
                            marker: {
                                radius: 2.5
                            }
                        }
                    }
                }
            }]
        }
    });

    // --- 桿狀圖 --- 

    var data = [{
        name: 'Austria',
        low: 69,
        high: 82
    }, {
        name: 'Belgium',
        low: 70,
        high: 81
    }, {
        name: 'Bulgaria',
        low: 69,
        high: 75
    }, {
        name: 'Croatia',
        low: 65,
        high: 78
    }, {
        name: 'Cyprus',
        low: 70,
        high: 81
    }, {
        name: 'Czech Republic',
        low: 70,
        high: 79
    }, {
        name: 'Denmark',
        low: 72,
        high: 81
    }, {
        name: 'Estonia',
        low: 68,
        high: 78
    }, {
        name: 'Finland',
        low: 69,
        high: 81
    }, {
        name: 'France',
        low: 70,
        high: 83
    }, {
        name: 'Greece',
        low: 68,
        high: 81
    }, {
        name: 'Spain',
        low: 69,
        high: 83
    }, {
        name: 'Netherlands',
        low: 73,
        high: 82
    }, {
        name: 'Ireland',
        low: 70,
        high: 82
    }, {
        name: 'Lithuania',
        low: 70,
        high: 75
    }, {
        name: 'Luxembourg',
        low: 68,
        high: 83
    }, {
        name: 'Latvia',
        low: 70,
        high: 75
    }, {
        name: 'Malta',
        low: 69,
        high: 82
    }, {
        name: 'Germany',
        low: 69,
        high: 81
    }, {
        name: 'Poland',
        low: 68,
        high: 78
    }, {
        name: 'Portugal',
        low: 63,
        high: 81
    }, {
        name: 'Romania',
        low: 66,
        high: 75
    }, {
        name: 'Slovakia',
        low: 70,
        high: 77
    }, {
        name: 'Slovenia',
        low: 69,
        high: 81
    }, {
        name: 'Sweden',
        low: 73,
        high: 82
    }, {
        name: 'Hungary',
        low: 68,
        high: 76
    }, {
        name: 'Italy',
        low: 69,
        high: 83
    }, {
        name: 'UK',
        low: 71,
        high: 81
    }];

    Highcharts.chart('container', {

        chart: {
            type: 'dumbbell',
            inverted: true
        },

        legend: {
            enabled: false
        },

        subtitle: {
            text: '1960 vs 2018'
        },

        title: {
            text: '桿狀圖標題'
        },

        tooltip: {
            shared: true
        },

        xAxis: {
            type: 'category'
        },

        yAxis: {
            title: {
                text: 'Life Expectancy (years)'
            }
        },

        series: [{
            name: 'Life expectancy change',
            data: data
        }]

    });

  

}) //JS尾端

function addnew(){
    var html = '<tr><td>'
+ '<select class="form-control">'
+ '<option>請選擇</option>'
+ '<option>1</option>'
+ '<option>2</option>'
+ '</select>'
    + '</td>'
    + '<td><input type="text" style="width: 100%;"></td>'
    + '<td>'
    +'<a href="javascript:void(0)"><i class="fas fa-arrow-circle-up"></i></a>&nbsp;'
    +'<a href="javascript:void(0)"><i class="fas fa-arrow-circle-down"></i></a>'
    +'</td>'
    + '<td>'
        + '<a href="javascript:void(0)" role="button" class="btn btn-shadow btn-color11-3111 btn-block" onclick="deleteadd(this)">'
        + '刪除 <i class="fas fa-trash-alt"></i>'
        + '</a>'
    + '</td>'
    + '<td>'
        + '<button role="button" class="btn btn-shadow btn-block btn-color11-3222" > 儲存 <i class="fas fa-save"></i></button>'
    + '</td>'
    + '</tr>';
    $('#addnew tr:last-child').after(html);
}
// onclick="$(\'#sup\').prop(\'disabled\',true)"

function deleteadd(a){
    a.parentNode.parentNode.remove();
}



function browse(){
    const { value: url } = Swal.fire({
        input: 'url',
        inputLabel: 'URL address',
        inputPlaceholder: 'Enter the URL'
      })
      
      if (url) {
        Swal.fire(`Entered URL: ${url}`)
      }
}



function upload(){
    const { value: file } = Swal.fire({
    title: '選擇您的檔案',
    input: 'file',
    inputAttributes: {
      'accept': 'image/*',
      'aria-label': 'Upload your profile picture'
    }
  })
  
  if (file) {
    const reader = new FileReader()
    reader.onload = (e) => {
      Swal.fire({
        title: 'Your uploaded picture',
        imageUrl: e.target.result,
        imageAlt: 'The uploaded picture'
      })
    }
    reader.readAsDataURL(file)
  }
}


function finish(){
    Swal.fire({
        // position: 'top-end',
        icon: 'success',
        title: '儲存成功',
        showConfirmButton: false,
        timer: 1500
      })
}
  


function search(id){
    $('#'+id).show();
}