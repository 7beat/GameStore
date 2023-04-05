tinymce.init({
    selector: 'textarea',
    plugins: 'lists code emoticons',
    toolbar: 'undo redo | styleselect | bold italic | ' +
        'alignleft aligncenter alignright alignjustify | ' +
        'outdent indent | numlist bullist | print | emoticons',
    browser_spellcheck: true,
    branding: false
});