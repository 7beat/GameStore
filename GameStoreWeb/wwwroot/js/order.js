var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    var status = url.includes("inprocess") ? "inprocess" :
        url.includes("completed") ? "completed" :
            url.includes("pending") ? "pending" :
                url.includes("approved") ? "approved" :
                    "all";
    loadDataTable(status);
});

function loadDataTable(status) {
    const dataTable = $('#tblData').DataTable({
        ajax: {
            url: `/Admin/Order/GetAll?status=${status}`
        },
        columns: [
            { data: "id", width: "15%" },
            { data: "name", width: "15%" },
            {
                data: "phoneNumber",
                width: "15%",
                render: (data, type, row) => {
                    return row.phoneNumber ? row.phoneNumber : "N/A";
                }
            },
            {
                data: null,
                width: "15%",
                render: (data, type, row) => {
                    const email = row.applicationUser ? row.applicationUser.email : row.guestEmailAddress;
                    return email;
                }
            },
            { data: "orderStatus", width: "15%" },
            { data: "orderTotal", width: "15%" },
            {
                data: "id",
                render: data => `
                    <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2">
                            <i class="bi bi-pencil-square"></i> Details
                        </a>
                    </div>
                `,
                width: "15%"
            },
        ]
    });
}




//function loadDataTable(status) {
//    const dataTable = $('#tblData').DataTable({
//        ajax: {
//            url: `/Admin/Order/GetAll?status=${status}`
//        },
//        columns: [
//            { data: "id", width: "15%" },
//            { data: "name", width: "15%" },
//            { data: "phoneNumber", width: "15%" },
//            {
//                data: "applicationUser.email",
//                width: "15%",
//                render: (data, type, row) => {
//                    // Check if email is null
//                    if (!data) {
//                        // Replace with GuestEmail
//                        return row.guestEmail;
//                    } else {
//                        return data;
//                    }
//                }
//            },
//            { data: "orderStatus", width: "15%" },
//            { data: "orderTotal", width: "15%" },
//            {
//                data: "id",
//                render: data => `
//                    <div class="w-75 btn-group" role="group">
//                        <a href="/Admin/Order/Details?orderId=${data}" class="btn btn-primary mx-2">
//                            <i class="bi bi-pencil-square"></i> Details
//                        </a>
//                    </div>
//                `,
//                width: "15%"
//            },
//        ]
//    });
//}
