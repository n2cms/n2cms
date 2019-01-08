var n2LoadingModal = (function () {
    
    function openModal(showSpinner, loadingMessage) {
        $('.btnLoadingModal').prop('disabled', true).addClass('disabled');

        showSpinner = showSpinner ? showSpinner : true;
        loadingMessage = loadingMessage ? loadingMessage : 'Loading...';

        var $body = $('<div/>');
        if (showSpinner)
            $body.append($('<i/>').addClass('fa fa-spinner fa-spin').css('font-size', '50px').css('margin-top', '30px'));
        
        $body.append($('<h3/>').text(loadingMessage));

        $('<div/>').addClass('modal').attr('role', 'dialog').append(
            $('<div/>').addClass('modal-dialog modal-sm').append(
                $('<div/>').addClass('modal-content').append(
                    $('<div/>').addClass('modal-body').css('text-align', 'center').append($body)
                )
            )
        ).modal({ backdrop: 'static', keyboard: false });
    }

    $('.btnLoadingModal').prop('disabled', false).removeClass('disabled');

    return {
        openModal: openModal
    }

}());