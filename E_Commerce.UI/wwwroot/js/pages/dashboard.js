document.addEventListener('DOMContentLoaded', function () {
    const conversionsEl = document.querySelector("#conversions");
    if (conversionsEl) {
        const radialOptions = {
            chart: {
                height: 292,
                type: 'radialBar',
            },
            plotOptions: {
                radialBar: {
                    startAngle: -135,
                    endAngle: 135,
                    dataLabels: {
                        name: {
                            fontSize: '14px',
                            offsetY: 100,
                            color: "#6c757d"
                        },
                        value: {
                            offsetY: 55,
                            fontSize: '20px',
                            formatter: val => val + "%",
                        }
                    },
                    track: {
                        background: "rgba(170,184,197, 0.2)",
                        margin: 0
                    },
                }
            },
            fill: {
                gradient: {
                    enabled: true,
                    shade: 'dark',
                    shadeIntensity: 0.2,
                    inverseColors: false,
                    opacityFrom: 1,
                    opacityTo: 1,
                    stops: [0, 50, 65, 91]
                },
            },
            stroke: { dashArray: 4 },
            colors: ["#ff6c2f", "#22c55e"],
            series: [65.2],
            labels: ['Returning Customer'],
            responsive: [{
                breakpoint: 380,
                options: { chart: { height: 180 } }
            }],
            grid: { padding: { top: 0, right: 0, bottom: 0, left: 0 } }
        };

        new ApexCharts(conversionsEl, radialOptions).render();
    }

    const performanceEl = document.querySelector("#dash-performance-chart");
    if (performanceEl) {
        const performanceOptions = {
            series: [
                {
                    name: "Page Views",
                    type: "bar",
                    data: [34, 65, 46, 68, 49, 61, 42, 44, 78, 52, 63, 67],
                },
                {
                    name: "Clicks",
                    type: "area",
                    data: [8, 12, 7, 17, 21, 11, 5, 9, 7, 29, 12, 35],
                },
            ],
            chart: {
                height: 313,
                type: "line",
                toolbar: { show: false },
            },
            stroke: {
                dashArray: [0, 0],
                width: [0, 2],
                curve: 'smooth'
            },
            fill: {
                opacity: [1, 1],
                type: ['solid', 'gradient'],
                gradient: {
                    type: "vertical",
                    inverseColors: false,
                    opacityFrom: 0.5,
                    opacityTo: 0,
                    stops: [0, 90]
                }
            },
            markers: {
                size: [0, 0],
                strokeWidth: 2,
                hover: { size: 4 },
            },
            xaxis: {
                categories: [
                    "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                    "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
                ],
                axisTicks: { show: false },
                axisBorder: { show: false },
            },
            yaxis: {
                min: 0,
                axisBorder: { show: false }
            },
            grid: {
                show: true,
                strokeDashArray: 3,
                xaxis: { lines: { show: false } },
                yaxis: { lines: { show: true } },
                padding: { top: 0, right: -2, bottom: 0, left: 10 },
            },
            legend: {
                show: true,
                horizontalAlign: "center",
                offsetY: 5,
                markers: {
                    width: 9,
                    height: 9,
                    radius: 6,
                },
                itemMargin: { horizontal: 10 },
            },
            plotOptions: {
                bar: {
                    columnWidth: "30%",
                    barHeight: "70%",
                    borderRadius: 3,
                }
            },
            colors: ["#ff6c2f", "#22c55e"],
            tooltip: {
                shared: true,
                y: [{
                    formatter: y => (typeof y !== "undefined" ? y.toFixed(1) + "k" : y)
                }, {
                    formatter: y => (typeof y !== "undefined" ? y.toFixed(1) + "k" : y)
                }]
            }
        };

        new ApexCharts(performanceEl, performanceOptions).render();
    }

    const mapEl = document.querySelector('#world-map-markers');
    if (mapEl && window.jsVectorMap) {
        new jsVectorMap({
            map: 'world',
            selector: '#world-map-markers',
            zoomOnScroll: true,
            zoomButtons: false,
            markersSelectable: true,
            markers: [
                { name: "Canada", coords: [56.1304, -106.3468] },
                { name: "Brazil", coords: [-14.2350, -51.9253] },
                { name: "Russia", coords: [61, 105] },
                { name: "China", coords: [35.8617, 104.1954] },
                { name: "United States", coords: [37.0902, -95.7129] }
            ],
            markerStyle: {
                initial: { fill: "#7f56da" },
                selected: { fill: "#22c55e" }
            },
            labels: {
                markers: { render: marker => marker.name }
            },
            regionStyle: {
                initial: {
                    fill: 'rgba(169,183,197, 0.3)',
                    fillOpacity: 1,
                }
            }
        });
    }
});
