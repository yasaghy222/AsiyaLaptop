/* ------------------------------------------------------------------------------
 *
 *  # Echarts - lines and areas
 *
 *  Lines and areas chart configurations
 *
 *  Version: 1.0
 *  Latest update: August 1, 2015
 *
 * ---------------------------------------------------------------------------- */
//select top orders
function TopOrders() {
    $.ajax({
        url: '/Base/TopOrders',
        type: 'POST',
        success: function (Resualt) {
            var source = $('#topOrderSource').html();
            var template = Handlebars.compile(source);
            var list = template({ orderList: Resualt });
            $('#tbl-TopOrder tbody').html(list);
        },
        error: function () {
            notifiction(3, 'عدم فراخوانی رکورد ها');
        }
    });
};
$(() => {
    const host = window.location.origin;

    // Set paths
    // ------------------------------

    require.config({
        paths: {
            echarts: '' + host + '/Areas/Admin/Scripts/Plugin/echarts'
        }
    });


    // Configuration
    // ------------------------------

    require(
        [
            'echarts',
            'echarts/theme/limitless',
            'echarts/chart/bar',
            'echarts/chart/line'
        ],


        // Charts setup
        function (ec, limitless) {

            // Initialize charts
            // ------------------------------

            var viewChart = ec.init(document.getElementById('view_chart'), limitless);

            // Charts setup
            // ------------------------------

            //
            // area options
            //

            viewChart_options = {

                // Setup grid
                grid: {
                    x: 40,
                    x2: 20,
                    y: 35,
                    y2: 25
                },

                // Add tooltip
                tooltip: {
                    trigger: 'axis'
                },

                // Enable drag recalculate
                calculable: true,

                // Horizontal axis
                xAxis: [{
                    type: 'category',
                    boundaryGap: true,
                    data: [
                        '1397-9-1', '1397-9-3', '1397-9-5', '1397-9-7', '1397-9-9'
                    ]
                }],

                // Vertical axis
                yAxis: [{
                    type: 'value'
                }],

                // Add series
                series: [
                    {
                        name: 'بازدید',
                        type: 'line',
                        smooth: true,
                        itemStyle: { normal: { areaStyle: { type: 'default' } } },
                        data: [50, 40, 10, 2, 3]
                    }
                ]
            };

            // Apply options
            // ------------------------------

            viewChart.setOption(viewChart_options);



            // Resize charts
            // ------------------------------

            window.onresize = function () {
                setTimeout(function () {
                    viewChart.resize();
                }, 200);
            }
        }
    );
});
