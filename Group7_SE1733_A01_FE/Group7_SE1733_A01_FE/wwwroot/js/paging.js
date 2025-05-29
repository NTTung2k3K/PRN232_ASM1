let ItemPerPage = 3; 
let currentPage = 1; 

function getTableRows() {
    const tableBody = document.getElementById('idTBody');
    return Array.from(tableBody.getElementsByTagName('tr'));
}

function calculateTotalPages(rows) {
    return Math.ceil(rows.length / ItemPerPage);
}

function displayRows(rows) {
    const start = (currentPage - 1) * ItemPerPage;
    const end = start + ItemPerPage;
    rows.forEach((row, index) => {
        row.style.display = (index >= start && index < end) ? '' : 'none';
    });
}

function createPaginationControls(totalPages) {
    const existingPagination = document.getElementById('pagination');
    if (existingPagination) {
        existingPagination.remove();
    }

    const paginationContainer = document.createElement('div');
    paginationContainer.id = 'pagination';
    paginationContainer.className = 'd-flex justify-content-center mt-3'; // Canh giữa

    const ul = document.createElement('ul');
    ul.className = 'pagination';

    if (currentPage > 1) {
        const prevLi = document.createElement('li');
        prevLi.className = 'page-item';
        const prevButton = document.createElement('button');
        prevButton.className = 'page-link';
        prevButton.textContent = 'Previous';
        prevButton.addEventListener('click', () => {
            currentPage--;
            updatePagination();
        });
        prevLi.appendChild(prevButton);
        ul.appendChild(prevLi);
    }

    for (let i = 1; i <= totalPages; i++) {
        const li = document.createElement('li');
        li.className = 'page-item' + (i === currentPage ? ' active' : '');
        const pageButton = document.createElement('button');
        pageButton.className = 'page-link';
        pageButton.textContent = i;
        pageButton.addEventListener('click', () => {
            currentPage = i;
            updatePagination();
        });
        li.appendChild(pageButton);
        ul.appendChild(li);
    }

    if (currentPage < totalPages) {
        const nextLi = document.createElement('li');
        nextLi.className = 'page-item';
        const nextButton = document.createElement('button');
        nextButton.className = 'page-link';
        nextButton.textContent = 'Next';
        nextButton.addEventListener('click', () => {
            currentPage++;
            updatePagination();
        });
        nextLi.appendChild(nextButton);
        ul.appendChild(nextLi);
    }

    paginationContainer.appendChild(ul);
    document.body.appendChild(paginationContainer);
}

function updatePagination() {
    const rows = getTableRows();
    const totalPages = calculateTotalPages(rows);
    displayRows(rows);
    createPaginationControls(totalPages);
}

updatePagination();