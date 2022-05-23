$(function () {
    $('form').submit( e => {
        e.preventDefault();

        const q = $('#search').val();

        $('tbody').load('/ratings/Search2?query='+q);
        
    })
    
});