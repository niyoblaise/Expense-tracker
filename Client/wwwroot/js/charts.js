
if (!window.__etChartDefaultsApplied) {
    if (window.Chart && Chart.defaults) {
        Chart.defaults.color = '#e6e6e6';
        Chart.defaults.font.family = 'Inter, system-ui, -apple-system, Segoe UI, Roboto, Arial, sans-serif';
        Chart.defaults.plugins.legend.labels = Chart.defaults.plugins.legend.labels || {};
        Chart.defaults.plugins.legend.labels.color = '#e6e6e6';
    }
    window.__etChartDefaultsApplied = true;
}

window.renderExpenseChart = (chartId, labels, data) => {
    const el = document.getElementById(chartId);
    if (!el) {
        console.warn(`[charts] canvas not found for id: ${chartId}`);
        return;
    }
    const ctx = el.getContext("2d");
    if (window[chartId]) window[chartId].destroy();
    window[chartId] = new Chart(ctx, {
        type: "pie",
        data: {
            labels: labels,
            datasets: [{
                label: "Breakdown",
                data: data,
                backgroundColor: [
                    "#ff7a00", "#ffc107", "#00c2ff", "#7c4dff", "#4caf50", "#e83e8c", "#03dac6", "#ff4081"
                ],
                borderColor: "#0f0f14",
                borderWidth: 2,
                hoverOffset: 8
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    labels: {
                        color: '#e6e6e6'
                    }
                }
            }
        }
    });
};
