$(function () {
    $('#tblItem').tGrid({
        searchable: true,
        addMode: "append",
        headerButtons: ['#open-item'],
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblItem_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);

                work.action = function (item, index, total) {
                    grid.addRow(Item.estrutura, item);

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblItem_count').val(grid.count());
        }
    });
});

var Item = {
    estrutura:
        '<tr class="data">' +
            '<td>${nome}</td>' +
            '<td style="text-align: right">${quantidade}</td>' +
         '</tr>'
};